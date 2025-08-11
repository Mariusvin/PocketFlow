import asyncio
import time
import uuid
from dataclasses import dataclass, field
from enum import Enum, auto

class JobStatus(Enum):
    PENDING = auto()
    RUNNING = auto()
    COMPLETED = auto()
    FAILED = auto()

@dataclass
class Job:
    job_id: str
    status: JobStatus = JobStatus.PENDING
    sse_queue: asyncio.Queue = field(default_factory=asyncio.Queue)
    result: dict | None = None
    created_at: float = field(default_factory=time.time)
    updated_at: float = field(default_factory=time.time)

class JobStore:
    def __init__(self, retention_time_seconds: int = 600):
        self._jobs: dict[str, Job] = {}
        self._retention_time_seconds = retention_time_seconds

    def create_job(self) -> Job:
        job_id = str(uuid.uuid4())
        job = Job(job_id=job_id)
        self._jobs[job_id] = job
        return job

    def get_job(self, job_id: str) -> Job | None:
        return self._jobs.get(job_id)

    def update_job_status(self, job_id: str, status: JobStatus):
        if job := self.get_job(job_id):
            job.status = status
            job.updated_at = time.time()
        else:
            raise ValueError(f"Job with id {job_id} not found.")

    def set_job_result(self, job_id: str, result: dict, success: bool):
        if job := self.get_job(job_id):
            job.result = result
            job.status = JobStatus.COMPLETED if success else JobStatus.FAILED
            job.updated_at = time.time()
            # Put a final message on the queue to unblock any listeners
            job.sse_queue.put_nowait({"step": "complete", "progress": 100, "data": result})
        else:
            raise ValueError(f"Job with id {job_id} not found.")

    def cleanup_old_jobs(self):
        current_time = time.time()
        jobs_to_delete = []
        for job_id, job in self._jobs.items():
            if job.status in [JobStatus.COMPLETED, JobStatus.FAILED]:
                if (current_time - job.updated_at) > self._retention_time_seconds:
                    jobs_to_delete.append(job_id)

        for job_id in jobs_to_delete:
            del self._jobs[job_id]

        if jobs_to_delete:
            print(f"Cleaned up {len(jobs_to_delete)} old jobs.")
