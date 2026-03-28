data class User(val id: Int, val name: String)

interface Repository<T> {
    fun findAll(): List<T>
    fun findById(id: Int): T?
    fun save(item: T)
    fun deleteById(id: Int): Boolean
}

class InMemoryUserRepository : Repository<User> {
    private val store = mutableListOf<User>()
    
    override fun findAll(): List<User> = store.toList()
    override fun findById(id: Int): User? = store.find { it.id == id }
    override fun save(item: User) { store.add(item) }
    override fun deleteById(id: Int): Boolean = store.removeAll { it.id == id }
}

fun main() {
    val repo = InMemoryUserRepository()
    repo.save(User(1, "Alice"))
    repo.save(User(2, "Bob"))
    println("Found: ${repo.findById(1)}")
    println("Total users: ${repo.findAll().size}")
}
