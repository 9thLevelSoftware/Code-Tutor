let todos = [];

todos.push('Buy groceries');
todos.push('Clean room');
todos.unshift('URGENT: Pay bills');

console.log('Todos:', todos);
// ['URGENT: Pay bills', 'Buy groceries', 'Clean room']

let completed = todos.shift();
console.log('Completed:', completed);
console.log('Remaining todos:', todos);
// ['Buy groceries', 'Clean room']