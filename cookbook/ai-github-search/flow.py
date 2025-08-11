from pocketflow import Flow
from nodes import BuildSearchQuery, SearchGitHub, RankAndFormat


def create_search_flow() -> Flow:
    q = BuildSearchQuery()
    s = SearchGitHub()
    r = RankAndFormat()
    q >> s >> r
    return Flow(start=q)


