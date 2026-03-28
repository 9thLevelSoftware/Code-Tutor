data class Todo(val id: Int, var title: String)

class TodoStore {
    private val items = mutableListOf<Todo>()
    private var nextId = 1
    
    fun create(title: String): Todo {
        TODO("Create and store a new Todo")
    }
    
    fun readAll(): List<Todo> = items.toList()
    
    fun update(id: Int, title: String): Boolean {
        TODO("Update the Todo with given id")
    }
    
    fun delete(id: Int): Boolean {
        TODO("Delete the Todo with given id")
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
