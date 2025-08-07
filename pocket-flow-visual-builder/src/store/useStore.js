import { create } from 'zustand';
import {
  applyNodeChanges,
  applyEdgeChanges,
} from 'react-flow';
import { v4 as uuidv4 } from 'uuid';

const useStore = create((set, get) => ({
  nodes: [],
  edges: [],

  onNodesChange: (changes) => {
    set({
      nodes: applyNodeChanges(changes, get().nodes),
    });
  },

  onEdgesChange: (changes) => {
    set({
      edges: applyEdgeChanges(changes, get().edges),
    });
  },

  addNode: (type, position) => {
    const newNode = {
      id: uuidv4(),
      type,
      position,
      data: { label: `${type} node` }, // Basic data
    };
    set({ nodes: [...get().nodes, newNode] });
  },
}));

export default useStore;
