from pocketflow import Flow, BatchFlow
from .nodes import TextNode, ImageNode, RouterNode, MergeNode, AITextGeneratorNode, WebsiteRendererNode

def create_component_processing_flow():
    """
    Creates a base flow for processing a single website component.
    This flow starts with a router that directs the component to the
    appropriate processor (Text, Image, or AI Text), and then merges the result.
    """
    # Create instances of the nodes for the base flow.
    router_node = RouterNode()
    text_node = TextNode()
    image_node = ImageNode()
    ai_text_node = AITextGeneratorNode()
    merge_node = MergeNode()

    # Define the processing path for a single component using the correct syntax.
    router_node - "text" >> text_node
    router_node - "image" >> image_node
    router_node - "ai_text" >> ai_text_node

    # The output of all processing nodes goes to the merge node.
    text_node >> merge_node
    image_node >> merge_node
    ai_text_node >> merge_node

    # The base flow starts with the router.
    return Flow(start=router_node)


class ComponentBatchFlow(BatchFlow):
    """
    A BatchFlow that orchestrates the processing of all website components.
    """
    def prep(self, shared):
        """
        Prepares the list of parameters for the batch execution.
        Each parameter will be a `component_key`.
        """
        print("BatchFlow: Preparing component keys for processing.")
        keys = list(shared.get("components", {}).keys())
        # BatchFlow expects a list of dictionaries, where each dictionary
        # maps to the parameters of the base flow's starting node.
        return [{"component_key": key} for key in keys]


def create_website_flow():
    """
    Creates the complete, final flow for building the website.
    """
    # 1. Create the base flow for processing one component.
    base_flow = create_component_processing_flow()

    # 2. Create the BatchFlow to run the base flow for all components.
    batch_flow = ComponentBatchFlow(start=base_flow)

    # 3. Create the final rendering node.
    renderer_node = WebsiteRendererNode()

    # 4. Connect the batch flow to the renderer. After the batch completes,
    # the flow will proceed to the renderer node.
    batch_flow >> renderer_node

    # 5. The overall flow starts with the batch process.
    final_flow = Flow(start=batch_flow)

    return final_flow
