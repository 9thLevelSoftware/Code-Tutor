// ========== commonMain/kotlin/di/CommonModule.kt ==========
import org.koin.dsl.module
import org.koin.core.module.dsl.viewModel
import org.koin.core.module.dsl.singleOf
import org.koin.core.module.dsl.bind

val commonModule = module {
    // Network - fully shared, no platform deps
    single { NetworkClient() }
    
    // Repository - implementation depends on database (provided by platform)
    singleOf(::NotesRepositoryImpl) { bind<NotesRepository>() }
    
    // ViewModel - shared
    viewModel { NotesViewModel(get()) }
}

// ========== androidMain/kotlin/di/PlatformModule.android.kt ==========
import android.content.Context
import app.cash.sqldelight.driver.android.AndroidSqliteDriver
import org.koin.dsl.module

val platformModule = module {
    // Android-specific database driver
    single {
        val context: Context = get()
        AndroidSqliteDriver(
            schema = AppDatabase.Schema,
            context = context,
            name = "notes.db"
        )
    }
    
    // Database instance using Android driver
    single { AppDatabase(get()) }
    
    // Android logger implementation
    single<PlatformLogger> { AndroidLogger() }
}

// ========== iosMain/kotlin/di/PlatformModule.ios.kt ==========
import platform.Foundation.*
import app.cash.sqldelight.driver.native.NativeSqliteDriver
import org.koin.dsl.module

val platformModule = module {
    // iOS-specific database driver
    single {
        NativeSqliteDriver(
            schema = AppDatabase.Schema,
            name = "notes.db"
        )
    }
    
    // Database instance using native driver
    single { AppDatabase(get()) }
    
    // iOS logger implementation
    single<PlatformLogger> { IOSLogger() }
}

// ========== Initialization ==========
// Android: initKoin(platformModule) { androidContext(this) }
// iOS: initKoin(platformModule)