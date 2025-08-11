from pocketflow import Flow
from nodes import (
    GenerateOutline, WriteContent, ApplyStyle,
    GenerateTests, ImplementSolution, RunAndValidate
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
    Create and configure the code generation workflow.
    """
    # Create node instances
    test_node = GenerateTests()
    impl_node = ImplementSolution()
    validate_node = RunAndValidate()

    # Connect nodes in sequence
    test_node >> impl_node >> validate_node

    # Create flow starting with the test generation node
    code_gen_flow = Flow(start=test_node)

    return code_gen_flow
