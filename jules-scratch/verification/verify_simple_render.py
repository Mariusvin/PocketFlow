from playwright.sync_api import sync_playwright, expect

def run(playwright):
    browser = playwright.chromium.launch(headless=True)
    page = browser.new_page()

    # Listen for console messages
    page.on("console", lambda msg: print(f"CONSOLE: {msg.text}"))

    try:
        page.goto("http://localhost:5174/")

        # Check for the "Hello, World!" heading
        heading = page.get_by_role("heading", name="Hello, World! This is a direct render.")
        expect(heading).to_be_visible(timeout=10000)

        print("Verification successful! Taking screenshot.")
        page.screenshot(path="jules-scratch/verification/simple_render_success.png")

    except Exception as e:
        print(f"An error occurred: {e}")
        print("Page content on error:")
        print(page.content())
        page.screenshot(path="jules-scratch/verification/simple_render_error.png")
    finally:
        browser.close()

with sync_playwright() as playwright:
    run(playwright)
