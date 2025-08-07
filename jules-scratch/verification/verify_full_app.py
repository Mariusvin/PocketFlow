from playwright.sync_api import sync_playwright, expect

def run(playwright):
    browser = playwright.chromium.launch(headless=True)
    page = browser.new_page()

    try:
        page.goto("http://localhost:5174/")

        # Use a more specific locator for the input
        repo_input = page.get_by_placeholder("https://github.com/user/repo")
        expect(repo_input).to_be_visible()
        repo_input.fill("https://github.com/pocket-flow/pocket-flow")

        # Use a role-based locator for the button
        submit_button = page.get_by_role("button", name="Start Q&A")
        submit_button.click()

        # Wait for navigation to the chat page and for the chat window to be ready
        expect(page).to_have_url(lambda url: '/chat/' in url, timeout=10000)

        # Wait for the "Ready" status and the initial AI message
        expect(page.get_by_text("Repository ready!")).to_be_visible(timeout=10000)

        # Take a screenshot of the chat interface
        page.screenshot(path="jules-scratch/verification/full_app_success.png")
        print("Verification successful!")

    except Exception as e:
        print(f"An error occurred: {e}")
        print("Page content on error:")
        print(page.content())
        page.screenshot(path="jules-scratch/verification/full_app_error.png")
    finally:
        browser.close()

with sync_playwright() as playwright:
    run(playwright)
