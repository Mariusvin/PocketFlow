from playwright.sync_api import sync_playwright, expect, TimeoutError

def run(playwright):
    browser = playwright.chromium.launch(headless=True)
    page = browser.new_page()

    # Listen for console messages and print them
    page.on("console", lambda msg: print(f"CONSOLE: {msg.text}"))

    try:
        print("Navigating to http://localhost:5174/")
        page.goto("http://localhost:5174/", wait_until="domcontentloaded")

        print("Waiting for #root to have a child element...")
        # This will wait for React to render something into the root div
        page.wait_for_selector("#root > *", timeout=15000)

        print("React has rendered. Page content:")
        print(page.content())

        # Check for the "Hello, World!" heading
        heading = page.get_by_role("heading", name="Hello, World! This is a direct render.")
        expect(heading).to_be_visible()

        print("Verification successful! Taking screenshot.")
        page.screenshot(path="jules-scratch/verification/success.png")

    except TimeoutError as e:
        print(f"TimeoutError: {e}")
        print("The page did not render the expected content in time. Here is the final page content:")
        print(page.content())
        page.screenshot(path="jules-scratch/verification/error.png")
    except Exception as e:
        print(f"An unexpected error occurred: {e}")
        page.screenshot(path="jules-scratch/verification/error.png")
    finally:
        browser.close()

with sync_playwright() as playwright:
    run(playwright)
