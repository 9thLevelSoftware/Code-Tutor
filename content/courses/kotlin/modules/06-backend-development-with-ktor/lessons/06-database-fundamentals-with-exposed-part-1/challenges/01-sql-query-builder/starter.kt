class QueryBuilder {
    private var columns = listOf<String>()
    private var table = ""
    private var condition = ""
    
    fun select(vararg cols: String): QueryBuilder {
        TODO("Store columns and return this")
    }
    
    fun from(tableName: String): QueryBuilder {
        TODO("Store table and return this")
    }
    
    fun where(cond: String): QueryBuilder {
        TODO("Store condition and return this")
    }
    
    fun build(): String {
        TODO("Assemble and return the SQL string")
    }
}

fun main() {
    val query = QueryBuilder()
        .select("name", "email")
        .from("users")
        .where("age > 18")
        .build()
    println(query)
}
