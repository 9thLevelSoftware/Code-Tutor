from sqlalchemy.ext.asyncio import create_async_engine, AsyncSession, async_sessionmaker
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column, relationship
from sqlalchemy import ForeignKey, String, select, func
from sqlalchemy.orm import selectinload
from fastapi import FastAPI, Depends, HTTPException
from pydantic import BaseModel
from datetime import datetime, timezone
from typing import List

# Database setup
DATABASE_URL = "sqlite+aiosqlite:///./tasks.db"

engine = create_async_engine(DATABASE_URL, echo=True)
async_session = async_sessionmaker(engine, expire_on_commit=False)

class Base(DeclarativeBase):
    pass

class Project(Base):
    __tablename__ = "projects"
    
    id: Mapped[int] = mapped_column(primary_key=True)
    name: Mapped[str] = mapped_column(String(100))
    description: Mapped[str | None] = mapped_column(String(500), nullable=True)
    created_at: Mapped[datetime] = mapped_column(default=datetime.now(timezone.utc))
    
    tasks: Mapped[List["Task"]] = relationship(
        back_populates="project",
        cascade="all, delete-orphan"
    )

class Task(Base):
    __tablename__ = "tasks"
    
    id: Mapped[int] = mapped_column(primary_key=True)
    title: Mapped[str] = mapped_column(String(200))
    completed: Mapped[bool] = mapped_column(default=False)
    created_at: Mapped[datetime] = mapped_column(default=datetime.now(timezone.utc))
    project_id: Mapped[int] = mapped_column(ForeignKey("projects.id"))
    
    project: Mapped["Project"] = relationship(back_populates="tasks")

# Pydantic Schemas
class ProjectCreate(BaseModel):
    name: str
    description: str | None = None

class TaskCreate(BaseModel):
    title: str

class TaskResponse(BaseModel):
    id: int
    title: str
    completed: bool
    
    class Config:
        from_attributes = True

class ProjectResponse(BaseModel):
    id: int
    name: str
    description: str | None
    tasks: List[TaskResponse]
    
    class Config:
        from_attributes = True

class ProjectSummary(BaseModel):
    id: int
    name: str
    task_count: int

async def get_db():
    async with async_session() as session:
        yield session

app = FastAPI()

@app.on_event("startup")
async def startup():
    async with engine.begin() as conn:
        await conn.run_sync(Base.metadata.create_all)

@app.post("/projects/", response_model=ProjectResponse)
async def create_project(
    project: ProjectCreate,
    db: AsyncSession = Depends(get_db)
):
    db_project = Project(**project.model_dump())
    db.add(db_project)
    await db.commit()
    await db.refresh(db_project)
    return db_project

@app.get("/projects/{project_id}", response_model=ProjectResponse)
async def get_project(
    project_id: int,
    db: AsyncSession = Depends(get_db)
):
    result = await db.execute(
        select(Project)
        .options(selectinload(Project.tasks))
        .where(Project.id == project_id)
    )
    project = result.scalar_one_or_none()
    if not project:
        raise HTTPException(status_code=404, detail="Project not found")
    return project

@app.post("/projects/{project_id}/tasks/", response_model=TaskResponse)
async def create_task(
    project_id: int,
    task: TaskCreate,
    db: AsyncSession = Depends(get_db)
):
    # Verify project exists
    result = await db.execute(
        select(Project).where(Project.id == project_id)
    )
    if not result.scalar_one_or_none():
        raise HTTPException(status_code=404, detail="Project not found")
    
    db_task = Task(**task.model_dump(), project_id=project_id)
    db.add(db_task)
    await db.commit()
    await db.refresh(db_task)
    return db_task

@app.put("/tasks/{task_id}/complete", response_model=TaskResponse)
async def complete_task(
    task_id: int,
    db: AsyncSession = Depends(get_db)
):
    result = await db.execute(
        select(Task).where(Task.id == task_id)
    )
    task = result.scalar_one_or_none()
    if not task:
        raise HTTPException(status_code=404, detail="Task not found")
    
    task.completed = True
    await db.commit()
    await db.refresh(task)
    return task

@app.get("/projects/", response_model=List[ProjectSummary])
async def list_projects(db: AsyncSession = Depends(get_db)):
    result = await db.execute(
        select(Project).options(selectinload(Project.tasks))
    )
    projects = result.scalars().all()
    return [
        ProjectSummary(
            id=p.id,
            name=p.name,
            task_count=len(p.tasks)
        )
        for p in projects
    ]

# Demonstration
if __name__ == "__main__":
    print("=== Task Manager API ===")
    print("\nEndpoints:")
    print("  POST /projects/              - Create project")
    print("  GET  /projects/{id}          - Get project with tasks")
    print("  POST /projects/{id}/tasks/   - Add task to project")
    print("  PUT  /tasks/{id}/complete    - Mark task done")
    print("  GET  /projects/              - List all with counts")
    print("\nRun with: uvicorn solution:app --reload")