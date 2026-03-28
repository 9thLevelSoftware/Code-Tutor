---
type: "EXAMPLE"
title: "Discriminated Unions - The Power Pattern"
---

Discriminated unions use a common 'tag' property with literal types to distinguish between union members. Combined with exhaustiveness checking, they're TypeScript's most powerful pattern:

```typescript
// THE DISCRIMINATED UNION PATTERN
// 1. Common property (discriminant/tag) with literal types
// 2. Each variant has unique literal value
// 3. TypeScript narrows automatically based on the discriminant

// Example: State machine for async operations
interface LoadingState {
  status: 'loading';  // Discriminant
}

interface SuccessState<T> {
  status: 'success';  // Discriminant
  data: T;
}

interface ErrorState {
  status: 'error';  // Discriminant
  error: string;
  code: number;
}

type AsyncState<T> = LoadingState | SuccessState<T> | ErrorState;

function renderState<T>(state: AsyncState<T>): string {
  switch (state.status) {
    case 'loading':
      // TypeScript knows: state is LoadingState
      return 'Loading...';
      
    case 'success':
      // TypeScript knows: state is SuccessState<T>
      // state.data is available!
      return `Data: ${JSON.stringify(state.data)}`;
      
    case 'error':
      // TypeScript knows: state is ErrorState
      // state.error and state.code are available!
      return `Error ${state.code}: ${state.error}`;
  }
}

let loading: AsyncState<string[]> = { status: 'loading' };
let success: AsyncState<string[]> = { status: 'success', data: ['a', 'b', 'c'] };
let error: AsyncState<string[]> = { status: 'error', error: 'Network failed', code: 500 };

console.log(renderState(loading));  // 'Loading...'
console.log(renderState(success));  // 'Data: ["a","b","c"]'
console.log(renderState(error));    // 'Error 500: Network failed'

// EXHAUSTIVENESS CHECKING with never
// If you add a new state and forget to handle it, TypeScript catches it!

interface IdleState {
  status: 'idle';
}

type FullAsyncState<T> = IdleState | LoadingState | SuccessState<T> | ErrorState;

function assertNever(x: never): never {
  throw new Error(`Unexpected value: ${x}`);
}

function handleFullState<T>(state: FullAsyncState<T>): string {
  switch (state.status) {
    case 'idle':
      return 'Ready to start';
    case 'loading':
      return 'Loading...';
    case 'success':
      return `Got: ${JSON.stringify(state.data)}`;
    case 'error':
      return `Failed: ${state.error}`;
    default:
      // If all cases are handled, state is 'never' here
      // If you add a new status and forget it, TypeScript errors!
      return assertNever(state);
  }
}

// Real-world: Redux-style actions
interface AddTodoAction {
  type: 'ADD_TODO';
  payload: { text: string; id: number };
}

interface ToggleTodoAction {
  type: 'TOGGLE_TODO';
  payload: { id: number };
}

interface DeleteTodoAction {
  type: 'DELETE_TODO';
  payload: { id: number };
}

interface ClearCompletedAction {
  type: 'CLEAR_COMPLETED';
  // No payload needed
}

type TodoAction = AddTodoAction | ToggleTodoAction | DeleteTodoAction | ClearCompletedAction;

interface Todo {
  id: number;
  text: string;
  completed: boolean;
}

function todoReducer(todos: Todo[], action: TodoAction): Todo[] {
  switch (action.type) {
    case 'ADD_TODO':
      // action.payload has text and id
      return [...todos, { 
        id: action.payload.id, 
        text: action.payload.text, 
        completed: false 
      }];
      
    case 'TOGGLE_TODO':
      // action.payload has id
      return todos.map(todo =>
        todo.id === action.payload.id
          ? { ...todo, completed: !todo.completed }
          : todo
      );
      
    case 'DELETE_TODO':
      // action.payload has id
      return todos.filter(todo => todo.id !== action.payload.id);
      
    case 'CLEAR_COMPLETED':
      // No payload to access
      return todos.filter(todo => !todo.completed);
      
    default:
      return assertNever(action);
  }
}

let initialTodos: Todo[] = [];

let afterAdd = todoReducer(initialTodos, { 
  type: 'ADD_TODO', 
  payload: { id: 1, text: 'Learn TypeScript' } 
});
console.log('After add:', afterAdd);
// After add: [{ id: 1, text: 'Learn TypeScript', completed: false }]

let afterToggle = todoReducer(afterAdd, { 
  type: 'TOGGLE_TODO', 
  payload: { id: 1 } 
});
console.log('After toggle:', afterToggle);
// After toggle: [{ id: 1, text: 'Learn TypeScript', completed: true }]
```
