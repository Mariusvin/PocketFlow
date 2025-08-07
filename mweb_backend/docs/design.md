# mWEB Project Design Document

This document outlines the design for mWEB, an AI-powered website builder based on the PocketFlow framework.

## 1. Requirements

mWEB will be a web application that allows users to build simple websites using a visual, drag-and-drop interface. The core features are:

- **Visual Editor:** A canvas where users can add, arrange, and configure website components.
- **Standard Components:** Basic web elements like Text Blocks, Images, and Containers (for layout).
- **AI-Powered Components ("mFlow"):** Special components that use AI to generate content.
    - **AI Text Generator:** Generates a block of text from a user-provided prompt.
    - **AI Image Generator:** Generates an image from a user-provided prompt.
- **One-Click Build:** A single action to process the visual design and generate a static HTML/CSS website.
- **Website Serving:** The generated static website will be hosted and viewable.

## 2. Architectural Design

The system will be composed of two primary parts: a frontend application for the user interface and a backend service for processing and rendering the website.

### Frontend
- **Framework:** A modern JavaScript framework like React or Vue.js.
- **Visual Canvas:** A library like `React Flow` or a similar tool will be used to create the node-based visual editor.
- **Functionality:**
    - The UI will allow users to construct a graph of website components.
    - It will provide forms for users to input content and AI prompts for each component.
    - On "build", the frontend will serialize the graph into a JSON object that represents the PocketFlow `Flow` and its initial `shared` store.
    - It will send this JSON to the backend API.

### Backend
- **Framework:** A Python web framework like FastAPI to create the API.
- **Core Logic:** The `pocketflow` library will be used to execute the website generation logic.
- **Functionality:**
    - Expose an API endpoint (e.g., `/api/build`) that accepts the flow JSON from the frontend.
    - Instantiate and run the PocketFlow graph.
    - The flow will execute nodes that fetch data, call AI models, and ultimately generate the website's static files (`index.html`, `style.css`).
    - Store the generated static files in a publicly accessible directory.
    - Return the URL of the generated site to the frontend.

## 3. Flow Diagram

The high-level data flow of the system can be visualized as follows:

```mermaid
graph TD
    subgraph Frontend (User's Browser)
        A[Visual Editor UI] -- User builds site --> B{Serialize Graph to JSON};
    end

    B -- HTTP POST Request --> C[/api/build endpoint];

    subgraph Backend (Server)
        C -- instantiates --> D[PocketFlow Graph];
        D -- executes --> E[AI Content Generation & Component Processing];
        E -- populates --> F[Shared Store];
        F -- is used by --> G[Website Renderer Node];
        G -- creates --> H[Static HTML/CSS Files];
    end

    C -- returns URL --> A;
    I[User's Browser] -- visits URL --> H;
```

## 4. Project Structure (Proposed)

```
mweb_project/
├── frontend/
│   ├── src/
│   └── package.json
└── backend/
    ├── main.py          # FastAPI app and API endpoint
    ├── flow.py          # Defines the PocketFlow graph structure
    ├── nodes.py         # Defines all component and AI nodes
    ├── utils/           # Helper functions for AI calls, rendering, etc.
    │   ├── llm_helper.py
    │   └── renderer.py
    ├── requirements.txt
    └── docs/
        └── design.md    # This file
```
