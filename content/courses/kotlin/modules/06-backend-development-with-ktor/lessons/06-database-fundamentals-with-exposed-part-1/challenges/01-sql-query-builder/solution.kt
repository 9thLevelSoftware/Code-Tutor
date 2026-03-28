class QueryBuilder {
    private var columns = listOf<String>()
    private var table = ""
    private var condition = ""
    
    fun select(vararg cols: String): QueryBuilder {
        columns = cols.toList()
        return this
    }
    
    fun from(tableName: String): QueryBuilder {
        table = tableName
        return this
    }
    
    fun where(cond: String): QueryBuilder {
        condition = cond
        return this
    }
    
    fun build(): String {
        val base = "SELECT ${columns.joinToString(", ")} FROM $table"
        return if (condition.isNotEmpty()) "$base WHERE $condition" else base
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
