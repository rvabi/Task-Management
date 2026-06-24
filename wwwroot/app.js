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
        const response = await fetch(`${API_BASE_URL}/api/tasks/search?name=${encodeURIComponent(keyword)}`);
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