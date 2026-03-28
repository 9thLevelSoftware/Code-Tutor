data class User(val id: Int, val name: String)

interface Repository<T> {
    fun findAll(): List<T>
    fun findById(id: Int): T?
    fun save(item: T)
    fun deleteById(id: Int): Boolean
}

class InMemoryUserRepository : Repository<User> {
    private val store = mutableListOf<User>()
    
    // TODO: Implement all interface methods
    override fun findAll(): List<User> = TODO()
    override fun findById(id: Int): User? = TODO()
    override fun save(item: User) = TODO()
    override fun deleteById(id: Int): Boolean = TODO()
}

fun main() {
    val repo = InMemoryUserRepository()
    repo.save(User(1, "Alice"))
    repo.save(User(2, "Bob"))
    println("Found: ${repo.findById(1)}")
    println("Total users: ${repo.findAll().size}")
}
