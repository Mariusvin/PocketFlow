from playwright.sync_api import sync_playwright, expect

def run(playwright):
    browser = playwright.chromium.launch(headless=True)
    page = browser.new_page()

    try:
        page.goto("http://localhost:5174/")

        # Check for the RepoInputForm heading
        heading = page.get_by_role("heading", name="Codebase Q&A")
        expect(heading).to_be_visible()

        # Take a screenshot
        page.screenshot(path="jules-scratch/verification/repo_input_form.png")

    finally:
        browser.close()

with sync_playwright() as playwright:
    run(playwright)
