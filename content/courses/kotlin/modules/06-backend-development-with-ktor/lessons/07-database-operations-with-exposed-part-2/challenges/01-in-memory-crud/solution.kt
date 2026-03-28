data class Todo(val id: Int, var title: String)

class TodoStore {
    private val items = mutableListOf<Todo>()
    private var nextId = 1
    
    fun create(title: String): Todo {
        val todo = Todo(nextId++, title)
        items.add(todo)
        return todo
    }
    
    fun readAll(): List<Todo> = items.toList()
    
    fun update(id: Int, title: String): Boolean {
        val todo = items.find { it.id == id } ?: return false
        todo.title = title
        return true
    }
    
    fun delete(id: Int): Boolean {
        return items.removeAll { it.id == id }
    }
}

fun main() {
    val store = TodoStore()
    store.create("Buy groceries")
    store.create("Walk the dog")
    println(store.readAll().first())
    store.update(1, "Buy organic groceries")
    println(store.readAll().first())
    store.delete(2)
    println("Remaining: ${store.readAll().size}")
}
