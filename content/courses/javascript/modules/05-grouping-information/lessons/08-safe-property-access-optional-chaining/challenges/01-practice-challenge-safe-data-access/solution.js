let userData = {
  name: 'Alice',
  age: 0,
  address: {
    street: '123 Main St'
  },
  hobbies: ['reading', 'coding']
};

let city = userData?.address?.city ?? 'Unknown';
let email = userData?.email ?? 'No email provided';
let age = userData?.age ?? 'Age not specified';
let firstHobby = userData?.hobbies?.[0] ?? 'No hobbies listed';

console.log('City:', city);  // 'Unknown'
console.log('Email:', email);  // 'No email provided'
console.log('Age:', age);  // 0 (not 'Age not specified'!)
console.log('First hobby:', firstHobby);  // 'reading'