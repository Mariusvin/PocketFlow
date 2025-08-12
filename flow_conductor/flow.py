from pocketflow import Flow
from nodes import (
    GenerateOutline, WriteContent, ApplyStyle,
    GenerateTests, ImplementSolution, RunAndValidate, ReviewAndCorrect
)

def create_article_generation_flow():
    """
    Create and configure the article writing workflow.
    """
    # Create node instances
    outline_node = GenerateOutline()
    content_node = WriteContent()
    style_node = ApplyStyle()

    # Connect nodes in sequence
    outline_node >> content_node >> style_node

    # Create flow starting with outline node
    article_flow = Flow(start=outline_node)

    return article_flow

def create_code_generation_flow():
    """
    Create and configure the agentic, self-correcting code generation workflow.
    """
    # Create node instances
    test_node = GenerateTests()
    impl_node = ImplementSolution()
    validate_node = RunAndValidate()
    correct_node = ReviewAndCorrect()

    # Define the flow graph
    test_node >> impl_node >> validate_node

    # The agentic loop
    validate_node - "failure" >> correct_node
    correct_node - "validate" >> validate_node

    # The "success" and "max_retries_reached" actions from validate_node will terminate the flow
    # because they are not explicitly connected to any other node.

    # Create flow starting with the test generation node
    code_gen_flow = Flow(start=test_node)

    return code_gen_flow
