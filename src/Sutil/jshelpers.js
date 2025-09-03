class DebugTrackedObject {
  constructor() {
    const err = new Error();
    const stackLines = err.stack.split("\n");
    // Skip constructor and Error itself
    this.creationSite = stackLines[2]?.trim();
    this.stack = stackLines;
  }
}

export function createDebugTrackedObject() { return new DebugTrackedObject(); }