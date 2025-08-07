from playwright.sync_api import sync_playwright, expect

def run(playwright):
    browser = playwright.chromium.launch(headless=True)
    page = browser.new_page()

    page.on("console", lambda msg: print(f"CONSOLE: {msg.text}"))

    try:
        print("Navigating to http://localhost:5174/")
        page.goto("http://localhost:5174/", wait_until="domcontentloaded")
        print("Navigation complete.")

        print("Waiting for #root > div")
        page.wait_for_selector("#root > div", timeout=15000)
        print("Selector found.")

        print("Page content:")
        print(page.content())

        # Check for the RepoInputForm heading
        heading = page.get_by_role("heading", name="Codebase Q&A")
        expect(heading).to_be_visible()

        print("Verification successful! Taking screenshot.")
        page.screenshot(path="jules-scratch/verification/success.png")

    except Exception as e:
        print(f"An error occurred: {e}")
        print("Page content on error:")
        print(page.content())
        page.screenshot(path="jules-scratch/verification/error.png")
    finally:
        browser.close()

with sync_playwright() as playwright:
    run(playwright)
