import asyncio
import json
from fastapi import FastAPI, BackgroundTasks, Form, Request
from fastapi.responses import StreamingResponse, FileResponse, JSONResponse
from fastapi.staticfiles import StaticFiles

from flow import create_article_generation_flow, create_code_generation_flow
from job_store import JobStore, JobStatus

from contextlib import asynccontextmanager

# Global instance of the JobStore
job_store = JobStore()

async def cleanup_task():
    while True:
        await asyncio.sleep(60) # Run every 60 seconds
        job_store.cleanup_old_jobs()

@asynccontextmanager
async def lifespan(app: FastAPI):
    # Start the background cleanup task
    task = asyncio.create_task(cleanup_task())
    yield
    # Clean up the task when the application shuts down
    task.cancel()

app = FastAPI(lifespan=lifespan)

# Mount static files
app.mount("/static", StaticFiles(directory="static"), name="static")

def run_workflow(job_id: str, workflow_type: str, user_input: str):
    """Run the selected workflow in the background"""
    try:
        job_store.update_job_status(job_id, JobStatus.RUNNING)
        job = job_store.get_job(job_id)

        if workflow_type == "article_generation":
            shared = {"topic": user_input, "sse_queue": job.sse_queue}
            flow = create_article_generation_flow()
        elif workflow_type == "code_generation":
            shared = {"problem_description": user_input, "sse_queue": job.sse_queue}
            flow = create_code_generation_flow()
        else:
            raise ValueError(f"Unknown workflow type: {workflow_type}")

        # The result of the flow run is the final shared state
        flow.run(shared)

        # The last message on the queue is the final result from the last node
        final_data = shared.get("validation_result", shared.get("final_article", {}))
        job_store.set_job_result(job_id, result={"final_data": final_data}, success=True)

    except Exception as e:
        # Ensure the job status is marked as FAILED
        error_result = {"error": str(e), "details": "The workflow failed unexpectedly."}
        job_store.set_job_result(job_id, result=error_result, success=False)


@app.post("/start-job")
async def start_job(
    background_tasks: BackgroundTasks,
    workflow_type: str = Form(...),
    user_input: str = Form(...)
):
    """Start a new job for a selected workflow"""
    job = job_store.create_job()
    background_tasks.add_task(run_workflow, job.job_id, workflow_type, user_input)
    return JSONResponse(content={"job_id": job.job_id, "status": "started"})

@app.get("/progress/{job_id}")
async def get_progress(request: Request, job_id: str):
    """Stream progress updates via SSE"""

    async def event_stream():
        job = job_store.get_job(job_id)
        if not job:
            yield f"data: {json.dumps({'error': 'Job not found'})}\n\n"
            return

        # First, send a connection confirmation message
        yield f"data: {json.dumps({'step': 'connected', 'progress': 0, 'data': {'message': f'Connected to job {job_id}'}})}\n\n"

        while True:
            # Check if the client has disconnected
            if await request.is_disconnected():
                break

            try:
                # Wait for the next message on the queue
                progress_msg = await asyncio.wait_for(job.sse_queue.get(), timeout=1.0)
                yield f"data: {json.dumps(progress_msg)}\n\n"

                # If the final message is sent, we can stop
                if progress_msg.get("step") == "complete":
                    break
            except asyncio.TimeoutError:
                # If there's a timeout, it means no new messages.
                # We can send a heartbeat or check the job status.
                # If the job is done, we break the loop.
                job = job_store.get_job(job_id) # Re-fetch job to get latest status
                if not job or job.status in [JobStatus.COMPLETED, JobStatus.FAILED]:
                    break
                else:
                     yield f"data: {json.dumps({'heartbeat': True})}\n\n"

    return StreamingResponse(event_stream(), media_type="text/event-stream")

@app.get("/")
async def get_index():
    """Serve the main page"""
    return FileResponse("static/index.html")

@app.get("/progress.html")
async def get_progress_page():
    """Serve the progress page"""
    return FileResponse("static/progress.html")

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)
