const API_BASE_URL = "";

function showMessage(message, isSuccess) {
    const messageDiv = document.getElementById("message");

    if (!messageDiv) return;

    messageDiv.innerHTML = message;
    messageDiv.className = isSuccess ? "success" : "error";
}

async function loadUsers() {
    const tableBody = document.getElementById("userTableBody");

    if (!tableBody) return;

    tableBody.innerHTML = "<tr><td colspan='4'>Loading users...</td></tr>";

    try {
        const response = await fetch(`${API_BASE_URL}/api/users`);
        const result = await response.json();

        tableBody.innerHTML = "";

        if (!result.success) {
            tableBody.innerHTML = `<tr><td colspan='4'>${result.message}</td></tr>`;
            return;
        }

        if (result.data.length === 0) {
            tableBody.innerHTML = "<tr><td colspan='4'>No users found.</td></tr>";
            return;
        }

        result.data.forEach(user => {
            const row = `
                <tr>
                    <td>${user.userId}</td>
                    <td>${user.userName}</td>
                    <td>${user.email}</td>
                    <td>
                        <a class="action-link" href="user-detail.html?id=${user.userId}">
                            View Tasks
                        </a>
                    </td>
                </tr>
            `;

            tableBody.innerHTML += row;
        });
    } catch (error) {
        tableBody.innerHTML = "<tr><td colspan='4'>Error loading users.</td></tr>";
    }
}

async function addUser() {
    const userName = document.getElementById("userName").value.trim();
    const email = document.getElementById("email").value.trim();

    if (userName === "" || email === "") {
        showMessage("User name and email are required.", false);
        return;
    }

    const user = {
        userName: userName,
        email: email
    };

    try {
        const response = await fetch(`${API_BASE_URL}/api/users`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(user)
        });

        const result = await response.json();

        if (result.success) {
            showMessage(result.message, true);

            document.getElementById("userName").value = "";
            document.getElementById("email").value = "";

            loadUsers();
        } else {
            showMessage(result.errors.join("<br>"), false);
        }
    } catch (error) {
        showMessage("Error creating user.", false);
    }
}

async function loadTasks() {
    const tableBody = document.getElementById("taskTableBody");

    if (!tableBody) return;

    tableBody.innerHTML = "<tr><td colspan='8'>Loading tasks...</td></tr>";

    try {
        const response = await fetch(`${API_BASE_URL}/api/tasks`);
        const result = await response.json();

        renderTasks(result);
    } catch (error) {
        tableBody.innerHTML = "<tr><td colspan='8'>Error loading tasks.</td></tr>";
    }
}

function renderTasks(result) {
    const tableBody = document.getElementById("taskTableBody");

    if (!tableBody) return;

    tableBody.innerHTML = "";

    if (!result.success) {
        tableBody.innerHTML = `<tr><td colspan='8'>${result.message}</td></tr>`;
        return;
    }

    if (result.data.length === 0) {
        tableBody.innerHTML = "<tr><td colspan='8'>No tasks found.</td></tr>";
        return;
    }

    result.data.forEach(task => {
        const createdDate = new Date(task.createdDate).toLocaleDateString();

        const row = `
            <tr>
                <td>${task.taskId}</td>
                <td>${task.title}</td>
                <td>${task.description ?? ""}</td>
                <td>${task.status}</td>
                <td>${task.userName}</td>
                <td>${createdDate}</td>
                <td>
                    <select class="status-select" onchange="changeStatus(${task.taskId}, this.value)">
                        <option value="Todo" ${task.status === "Todo" ? "selected" : ""}>Todo</option>
                        <option value="In Progress" ${task.status === "In Progress" ? "selected" : ""}>In Progress</option>
                        <option value="Done" ${task.status === "Done" ? "selected" : ""}>Done</option>
                    </select>
                </td>
                <td>
                    <a class="edit-link" href="edit-task.html?id=${task.taskId}">Edit</a>
                    <button class="delete-btn" onclick="deleteTask(${task.taskId})">Delete</button>
                </td>
            </tr>
        `;

        tableBody.innerHTML += row;
    });
}

async function searchTasks() {
    const keyword = document.getElementById("searchInput").value.trim();

    if (keyword === "") {
        loadTasks();
        return;
    }

    try {
        const response = await fetch(`/api/tasks/search?name=${encodeURIComponent(keyword)}`);
        const result = await response.json();

        renderTasks(result);
    } catch (error) {
        showMessage("Error searching tasks.", false);
    }
}

function clearSearch() {
    document.getElementById("searchInput").value = "";
    loadTasks();
}

async function changeStatus(taskId, status) {
    try {
        const response = await fetch(`${API_BASE_URL}/api/tasks/${taskId}/status`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                status: status
            })
        });

        const result = await response.json();

        if (result.success) {
            showMessage(result.message, true);
            loadTasks();
        } else {
            showMessage(result.errors.join("<br>"), false);
            loadTasks();
        }
    } catch (error) {
        showMessage("Error changing task status.", false);
        loadTasks();
    }
}

async function deleteTask(taskId) {
    const confirmDelete = confirm("Are you sure you want to delete this task?");

    if (!confirmDelete) {
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/api/tasks/${taskId}`, {
            method: "DELETE"
        });

        const result = await response.json();

        if (result.success) {
            showMessage(result.message, true);
            loadTasks();
        } else {
            showMessage(result.errors.join("<br>"), false);
        }
    } catch (error) {
        showMessage("Error deleting task.", false);
    }
}

async function populateUserDropdown() {
    const userDropdown = document.getElementById("userId");

    if (!userDropdown) return;

    userDropdown.innerHTML = "<option value=''>Loading users...</option>";

    try {
        const response = await fetch(`${API_BASE_URL}/api/users`);
        const result = await response.json();

        userDropdown.innerHTML = "<option value=''>Select user</option>";

        if (!result.success) {
            userDropdown.innerHTML = "<option value=''>Error loading users</option>";
            return;
        }

        if (result.data.length === 0) {
            userDropdown.innerHTML = "<option value=''>No users found</option>";
            return;
        }

        result.data.forEach(user => {
            const option = document.createElement("option");
            option.value = user.userId;
            option.textContent = `${user.userName} (${user.email})`;
            userDropdown.appendChild(option);
        });

    } catch (error) {
        userDropdown.innerHTML = "<option value=''>Error loading users</option>";
    }
}

async function addTask() {
    const title = document.getElementById("title").value.trim();
    const description = document.getElementById("description").value.trim();
    const status = document.getElementById("status").value;
    const userId = document.getElementById("userId").value;

    if (title === "") {
        showMessage("Task title is required.", false);
        return;
    }

    if (userId === "") {
        showMessage("Please select a user.", false);
        return;
    }

    const task = {
        title: title,
        description: description,
        status: status,
        userId: parseInt(userId)
    };

    try {
        const response = await fetch(`${API_BASE_URL}/api/tasks`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(task)
        });

        const result = await response.json();

        if (result.success) {
            showMessage(result.message, true);

            document.getElementById("title").value = "";
            document.getElementById("description").value = "";
            document.getElementById("status").value = "Todo";
            document.getElementById("userId").value = "";

            setTimeout(() => {
                window.location.href = "tasks.html";
            }, 1000);
        } else {
            showMessage(result.errors.join("<br>"), false);
        }

    } catch (error) {
        showMessage("Error creating task.", false);
    }
}
function getQueryStringValue(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}

async function loadEditTaskPage() {
    const taskId = getQueryStringValue("id");

    if (!taskId) {
        showMessage("Task ID is missing. Please open this page from the task list Edit button.", false);
        return;
    }

    const taskIdInput = document.getElementById("taskId");
    if (taskIdInput) {
        taskIdInput.value = taskId;
    }

    await populateEditUserDropdown();
    await getTaskById(taskId);
}

async function populateEditUserDropdown() {
    const userDropdown = document.getElementById("editUserId");

    if (!userDropdown) return;

    userDropdown.innerHTML = "<option value=''>Loading users...</option>";

    try {
        const response = await fetch(`${API_BASE_URL}/api/users`);
        const result = await response.json();

        userDropdown.innerHTML = "<option value=''>Select user</option>";

        if (!result.success) {
            userDropdown.innerHTML = "<option value=''>Error loading users</option>";
            return;
        }

        result.data.forEach(user => {
            const option = document.createElement("option");
            option.value = user.userId;
            option.textContent = `${user.userName} (${user.email})`;
            userDropdown.appendChild(option);
        });

    } catch (error) {
        userDropdown.innerHTML = "<option value=''>Error loading users</option>";
        showMessage("Error loading users.", false);
    }
}

async function getTaskById(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/api/tasks/${id}`);
        const result = await response.json();

        if (!result.success) {
            showMessage(result.message, false);
            return;
        }

        const task = result.data;

        document.getElementById("editTitle").value = task.title;
        document.getElementById("editDescription").value = task.description ?? "";
        document.getElementById("editStatus").value = task.status;
        document.getElementById("editUserId").value = task.userId;

    } catch (error) {
        showMessage("Error loading task details.", false);
    }
}

async function updateTask() {
    const taskId = document.getElementById("taskId").value;
    const title = document.getElementById("editTitle").value.trim();
    const description = document.getElementById("editDescription").value.trim();
    const status = document.getElementById("editStatus").value;
    const userId = document.getElementById("editUserId").value;

    if (title === "") {
        showMessage("Task title is required.", false);
        return;
    }

    if (userId === "") {
        showMessage("Please select a user.", false);
        return;
    }

    const task = {
        title: title,
        description: description,
        status: status,
        userId: parseInt(userId)
    };

    try {
        const response = await fetch(`${API_BASE_URL}/api/tasks/${taskId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(task)
        });

        const result = await response.json();

        if (result.success) {
            showMessage(result.message, true);

            setTimeout(() => {
                window.location.href = "tasks.html";
            }, 1000);
        } else {
            showMessage(result.errors.join("<br>"), false);
        }

    } catch (error) {
        showMessage("Error updating task.", false);
    }
}

async function loadDashboard() {
    const totalTasksElement = document.getElementById("totalTasks");
    const todoTasksElement = document.getElementById("todoTasks");
    const inProgressTasksElement = document.getElementById("inProgressTasks");
    const doneTasksElement = document.getElementById("doneTasks");
    const dashboardTaskBody = document.getElementById("dashboardTaskBody");

    if (!totalTasksElement) return;

    try {
        const response = await fetch("/api/tasks");
        const result = await response.json();

        if (!result.success) {
            showMessage(result.message, false);
            return;
        }

        const tasks = result.data;

        const totalTasks = tasks.length;
        const todoTasks = tasks.filter(task => task.status === "Todo").length;
        const inProgressTasks = tasks.filter(task => task.status === "In Progress").length;
        const doneTasks = tasks.filter(task => task.status === "Done").length;

        totalTasksElement.textContent = totalTasks;
        todoTasksElement.textContent = todoTasks;
        inProgressTasksElement.textContent = inProgressTasks;
        doneTasksElement.textContent = doneTasks;

        dashboardTaskBody.innerHTML = "";

        if (tasks.length === 0) {
            dashboardTaskBody.innerHTML = "<tr><td colspan='5'>No tasks found.</td></tr>";
            return;
        }

        const recentTasks = tasks.slice(0, 5);

        recentTasks.forEach(task => {
            const createdDate = new Date(task.createdDate).toLocaleDateString();

            const row = `
                <tr>
                    <td>${task.taskId}</td>
                    <td>${task.title}</td>
                    <td>${task.status}</td>
                    <td>${task.userName}</td>
                    <td>${createdDate}</td>
                </tr>
            `;

            dashboardTaskBody.innerHTML += row;
        });

    } catch (error) {
        showMessage("Error loading dashboard.", false);

        if (dashboardTaskBody) {
            dashboardTaskBody.innerHTML = "<tr><td colspan='5'>Error loading recent tasks.</td></tr>";
        }
    }
}

async function loadUserDetailPage() {
    const userId = getQueryStringValue("id");

    if (!userId) {
        showMessage("User ID is missing. Please open this page from Users page.", false);
        return;
    }

    await getUserWithTasks(userId);
}

async function getUserWithTasks(id) {
    const taskTableBody = document.getElementById("userTaskTableBody");

    if (taskTableBody) {
        taskTableBody.innerHTML = "<tr><td colspan='5'>Loading tasks...</td></tr>";
    }

    try {
        const response = await fetch(`/api/users/${id}/tasks`);
        const result = await response.json();

        if (!result.success) {
            showMessage(result.message, false);

            if (taskTableBody) {
                taskTableBody.innerHTML = "<tr><td colspan='5'>No data found.</td></tr>";
            }

            return;
        }

        const user = result.data;

        document.getElementById("detailUserId").textContent = user.userId;
        document.getElementById("detailUserName").textContent = user.userName;
        document.getElementById("detailEmail").textContent = user.email;

        renderUserTasks(user.tasks);

    } catch (error) {
        showMessage("Error loading user details.", false);

        if (taskTableBody) {
            taskTableBody.innerHTML = "<tr><td colspan='5'>Error loading tasks.</td></tr>";
        }
    }
}

function renderUserTasks(tasks) {
    const taskTableBody = document.getElementById("userTaskTableBody");

    if (!taskTableBody) return;

    taskTableBody.innerHTML = "";

    if (!tasks || tasks.length === 0) {
        taskTableBody.innerHTML = "<tr><td colspan='5'>This user has no assigned tasks.</td></tr>";
        return;
    }

    tasks.forEach(task => {
        const createdDate = new Date(task.createdDate).toLocaleDateString();

        const row = `
            <tr>
                <td>${task.taskId}</td>
                <td>${task.title}</td>
                <td>${task.description ?? ""}</td>
                <td>${task.status}</td>
                <td>${createdDate}</td>
            </tr>
        `;

        taskTableBody.innerHTML += row;
    });
}

function setThemeIcon() {
    const themeIcon = document.getElementById("themeIcon");

    if (!themeIcon) return;

    if (document.body.classList.contains("dark")) {
        themeIcon.textContent = "☀";
    } else {
        themeIcon.textContent = "☾";
    }
}

function toggleTheme() {
    document.body.classList.toggle("dark");

    if (document.body.classList.contains("dark")) {
        localStorage.setItem("theme", "dark");
    } else {
        localStorage.setItem("theme", "light");
    }

    setThemeIcon();
}

window.addEventListener("DOMContentLoaded", () => {
    const savedTheme = localStorage.getItem("theme");

    if (savedTheme === "dark") {
        document.body.classList.add("dark");
    }

    setThemeIcon();
});