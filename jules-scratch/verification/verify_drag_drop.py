from playwright.sync_api import sync_playwright, expect

def run_verification():
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        page = browser.new_page()

        # 1. Navigate to the app
        try:
            page.goto("http://localhost:5173")
        except Exception as e:
            print("Failed to connect to the server. Is it running on http://localhost:5173?")
            print(f"Error: {e}")
            browser.close()
            return

        # 2. Find the source element to drag
        # Using get_by_text which is a robust locator
        source_node = page.get_by_text("Summarizer Agent")
        expect(source_node).to_be_visible(timeout=10000) # Wait for app to load

        # 3. Find the target element to drop onto
        # The target is the react-flow canvas
        target_canvas = page.locator(".reactflow-wrapper")
        expect(target_canvas).to_be_visible()

        # 4. Perform the drag and drop
        source_node.drag_to(target_canvas)

        # 5. Assert that the new node was created
        # The default node data creates a label with "{type} node"
        new_node = page.get_by_text("summarizer node")
        expect(new_node).to_be_visible()

        # 6. Take a screenshot for visual confirmation
        page.screenshot(path="jules-scratch/verification/verification.png")

        print("Verification script completed successfully.")
        browser.close()

if __name__ == "__main__":
    run_verification()
