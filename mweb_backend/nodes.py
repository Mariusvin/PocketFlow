import os
from pocketflow import Node
from .utils.llm_helper import call_llm
from .utils.renderer import render_html_page

# This shared store structure is assumed based on the design document.
# Example:
# shared = {
#     "config": { ... },
#     "components": { ... },
#     "current_component_key": "c1_intro" # Used to pass data between nodes
# }

class TextNode(Node):
    """
    Processes a text component and generates an HTML paragraph.
    It reads the component key from the shared store.
    """
    def prep(self, shared):
        component_key = shared.get("current_component_key")
        return shared.get("components", {}).get(component_key, {})

    def exec(self, component_data):
        content = component_data.get("content", "Default text")
        print(f"Rendering Text: {content}")
        return f"<p>{content}</p>"

class ImageNode(Node):
    """
    Processes an image component and generates an HTML image tag.
    It reads the component key from the shared store.
    """
    def prep(self, shared):
        component_key = shared.get("current_component_key")
        return shared.get("components", {}).get(component_key, {})

    def exec(self, component_data):
        src = component_data.get("src", "https://via.placeholder.com/150")
        alt = component_data.get("alt", "placeholder image")
        print(f"Rendering Image: {src}")
        return f'<img src="{src}" alt="{alt}">'

class AITextGeneratorNode(Node):
    """
    Generates text using an LLM based on a prompt.
    """
    def prep(self, shared):
        component_key = shared.get("current_component_key")
        return shared.get("components", {}).get(component_key, {})

    def exec(self, component_data):
        prompt = component_data.get("prompt", "Please write a short, generic paragraph.")
        generated_text = call_llm(prompt)
        # The final output should still be a renderable HTML snippet.
        return f"<p>{generated_text}</p>"

class RouterNode(Node):
    """
    Inspects a component and decides which node to route to next.
    It receives the component_key from the BatchFlow via `self.params`.
    """
    def prep(self, shared):
        # 1. Get the component key from `self.params`.
        component_key = self.params.get('component_key')
        if not component_key:
            return None, "unsupported" # Should not happen in batch flow

        # 2. Get the component type for routing.
        component_type = shared.get("components", {}).get(component_key, {}).get("type")

        # 3. Return the key and type for post-processing.
        return component_key, component_type

    def exec(self, prep_res):
        # This node's main job is routing, so exec is a passthrough.
        # The key is passed to post() via prep_res.
        return prep_res

    def post(self, shared, prep_res, exec_res):
        if not prep_res:
             return "unsupported"

        component_key, component_type = prep_res

        # VERY IMPORTANT: Place the key in the shared store so the next node can use it.
        shared["current_component_key"] = component_key

        if component_type == "text":
            return "text"
        elif component_type == "image":
            return "image"
        elif component_type == "ai_text":
            return "ai_text"
        else:
            return "unsupported"

class MergeNode(Node):
    """
    Collects the generated HTML snippets for each component.
    The result of the previous node's exec() is passed to this node's prep().
    This is incorrect, data is passed via shared store.
    The previous node's exec() result is passed to this node's _exec() via prep().
    This node's prep() should get the data from the previous node's post().
    The `post` method of the previous node returns an action string, not data.
    The data must be passed via the shared store.

    Let's re-think. The `exec` of TextNode/ImageNode returns the HTML snippet.
    The `post` of TextNode/ImageNode does nothing.
    The `_run` of TextNode/ImageNode returns the result of `post`.
    The `Flow` uses this return value to find the next node.
    So, the HTML snippet is NOT passed to the MergeNode.

    The MergeNode needs to get the HTML snippet from the shared store.
    The TextNode/ImageNode's post() method needs to store the HTML snippet.
    Let's add that.
    """
    def prep(self, shared):
        # This node is called after a component has been processed.
        # It reads the last generated HTML from the shared store.
        last_html = shared.get("last_html_snippet", "")
        if "rendered_html" not in shared:
            shared["rendered_html"] = []
        return last_html

    def exec(self, component_html):
        print(f"Merging HTML snippet: {component_html[:30]}...")
        return component_html

    def post(self, shared, prep_res, exec_res):
        shared["rendered_html"].append(exec_res)

# We need to modify TextNode and ImageNode to store their output in the shared store.
def add_post_to_node(node_class):
    def post(self, shared, prep_res, exec_res):
        shared['last_html_snippet'] = exec_res
        return "default" # This assumes the default transition goes to the MergeNode
    node_class.post = post
    return node_class

class WebsiteRendererNode(Node):
    """
    Takes the collected HTML snippets and renders a full HTML page,
    saving it to a file.
    """
    def prep(self, shared):
        title = shared.get("config", {}).get("title", "My mWEB Page")
        body_content = shared.get("rendered_html", [])
        return title, body_content

    def exec(self, prep_res):
        title, body_content = prep_res
        print(f"Rendering full HTML page with title: '{title}'")
        return render_html_page(title, body_content)

    def post(self, shared, prep_res, exec_res):
        # Place output inside the project directory to keep the root clean.
        output_dir = "mweb_backend/output"
        os.makedirs(output_dir, exist_ok=True)

        filepath = os.path.join(output_dir, "index.html")
        with open(filepath, "w") as f:
            f.write(exec_res)

        print(f"Website saved to {filepath}")
        shared['final_filepath'] = filepath

TextNode = add_post_to_node(TextNode)
ImageNode = add_post_to_node(ImageNode)
AITextGeneratorNode = add_post_to_node(AITextGeneratorNode)
