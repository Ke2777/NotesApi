const apiUrl = "/notes";
const authUrl = "/auth";
const storageKey = "notes-auth";

const authPanel = document.querySelector("#authPanel");
const notesApp = document.querySelector("#notesApp");
const sessionBar = document.querySelector("#sessionBar");
const userBadge = document.querySelector("#userBadge");
const logoutButton = document.querySelector("#logoutButton");

const authForm = document.querySelector("#authForm");
const loginTab = document.querySelector("#loginTab");
const registerTab = document.querySelector("#registerTab");
const usernameInput = document.querySelector("#usernameInput");
const emailInput = document.querySelector("#emailInput");
const passwordInput = document.querySelector("#passwordInput");
const authSubmitButton = document.querySelector("#authSubmitButton");
const authStatus = document.querySelector("#authStatus");

const form = document.querySelector("#noteForm");
const titleInput = document.querySelector("#titleInput");
const contentInput = document.querySelector("#contentInput");
const submitButton = document.querySelector("#submitButton");
const cancelButton = document.querySelector("#cancelButton");
const refreshButton = document.querySelector("#refreshButton");
const notesGrid = document.querySelector("#notesGrid");
const emptyState = document.querySelector("#emptyState");
const notesCounter = document.querySelector("#notesCounter");
const statusText = document.querySelector("#statusText");

let authMode = "login";
let session = loadSession();
let notes = [];
let editingId = null;

function loadSession() {
    const raw = localStorage.getItem(storageKey);

    if (!raw) {
        return null;
    }

    try {
        return JSON.parse(raw);
    } catch {
        localStorage.removeItem(storageKey);
        return null;
    }
}

function saveSession(nextSession) {
    session = nextSession;
    localStorage.setItem(storageKey, JSON.stringify(nextSession));
}

function clearSession() {
    session = null;
    localStorage.removeItem(storageKey);
}

function setStatus(element, message, isError = false) {
    element.textContent = message;
    element.classList.toggle("error", isError);
}

function authHeaders() {
    return {
        "Authorization": `Bearer ${session.token}`
    };
}

function setAuthMode(mode) {
    authMode = mode;
    const isRegister = mode === "register";

    loginTab.classList.toggle("active", !isRegister);
    registerTab.classList.toggle("active", isRegister);
    usernameInput.hidden = !isRegister;
    usernameInput.required = isRegister;
    authSubmitButton.textContent = isRegister ? "Создать аккаунт" : "Войти";
    passwordInput.autocomplete = isRegister ? "new-password" : "current-password";
    setStatus(authStatus, "");
}

function renderSession() {
    const isAuthorized = session !== null;

    authPanel.hidden = isAuthorized;
    notesApp.hidden = !isAuthorized;
    sessionBar.hidden = !isAuthorized;

    if (!isAuthorized) {
        notes = [];
        renderNotes();
        return;
    }

    userBadge.textContent = `${session.user.username} · ${session.user.email}`;
    loadNotes();
}

function resetForm() {
    editingId = null;
    form.reset();
    submitButton.textContent = "Создать";
    cancelButton.hidden = true;
    setStatus(statusText, "");
}

function renderNotes() {
    notesCounter.textContent = notes.length;
    emptyState.hidden = notes.length > 0;
    notesGrid.innerHTML = "";

    for (const note of notes) {
        const card = document.createElement("article");
        card.className = "note-card";

        const title = document.createElement("h3");
        title.textContent = note.title;

        const content = document.createElement("p");
        content.textContent = note.content;

        const footer = document.createElement("div");
        footer.className = "note-footer";

        const id = document.createElement("span");
        id.className = "note-id";
        id.textContent = `#${note.id}`;

        const actions = document.createElement("div");
        actions.className = "note-actions";

        const editButton = document.createElement("button");
        editButton.type = "button";
        editButton.title = "Редактировать";
        editButton.textContent = "✎";
        editButton.addEventListener("click", () => startEdit(note));

        const deleteButton = document.createElement("button");
        deleteButton.type = "button";
        deleteButton.title = "Удалить";
        deleteButton.className = "delete";
        deleteButton.textContent = "×";
        deleteButton.addEventListener("click", () => deleteNote(note.id));

        actions.append(editButton, deleteButton);
        footer.append(id, actions);
        card.append(title, content, footer);
        notesGrid.append(card);
    }
}

async function requestJson(url, options = {}) {
    const response = await fetch(url, options);

    if (response.status === 401) {
        clearSession();
        renderSession();
        throw new Error("Unauthorized");
    }

    if (!response.ok) {
        const text = await response.text();
        throw new Error(text || `${options.method ?? "GET"} ${url} returned ${response.status}`);
    }

    if (response.status === 204) {
        return null;
    }

    return response.json();
}

async function submitAuth(event) {
    event.preventDefault();
    setStatus(authStatus, "Проверяем...");

    const payload = {
        email: emailInput.value.trim(),
        password: passwordInput.value
    };

    if (authMode === "register") {
        payload.username = usernameInput.value.trim();
    }

    try {
        const response = await requestJson(`${authUrl}/${authMode === "register" ? "register" : "login"}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        saveSession(response);
        authForm.reset();
        renderSession();
    } catch (error) {
        setStatus(authStatus, authMode === "register" ? "Не удалось создать аккаунт." : "Неверный email или пароль.", true);
        console.error(error);
    }
}

async function loadNotes() {
    setStatus(statusText, "Загрузка...");

    try {
        notes = await requestJson(apiUrl, {
            headers: authHeaders()
        });

        renderNotes();
        setStatus(statusText, "");
    } catch (error) {
        setStatus(statusText, "Не удалось загрузить заметки.", true);
        console.error(error);
    }
}

function startEdit(note) {
    editingId = note.id;
    titleInput.value = note.title;
    contentInput.value = note.content;
    submitButton.textContent = "Сохранить";
    cancelButton.hidden = false;
    titleInput.focus();
}

async function saveNote(event) {
    event.preventDefault();

    const payload = {
        title: titleInput.value.trim(),
        content: contentInput.value.trim()
    };

    if (payload.title.length < 3) {
        setStatus(statusText, "Заголовок должен быть минимум 3 символа.", true);
        return;
    }

    const url = editingId === null ? apiUrl : `${apiUrl}/${editingId}`;
    const method = editingId === null ? "POST" : "PUT";

    try {
        await requestJson(url, {
            method,
            headers: {
                ...authHeaders(),
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        resetForm();
        await loadNotes();
    } catch (error) {
        setStatus(statusText, "Не удалось сохранить заметку.", true);
        console.error(error);
    }
}

async function deleteNote(id) {
    try {
        await requestJson(`${apiUrl}/${id}`, {
            method: "DELETE",
            headers: authHeaders()
        });

        if (editingId === id) {
            resetForm();
        }

        await loadNotes();
    } catch (error) {
        setStatus(statusText, "Не удалось удалить заметку.", true);
        console.error(error);
    }
}

function logout() {
    clearSession();
    resetForm();
    renderSession();
}

authForm.addEventListener("submit", submitAuth);
loginTab.addEventListener("click", () => setAuthMode("login"));
registerTab.addEventListener("click", () => setAuthMode("register"));
logoutButton.addEventListener("click", logout);
form.addEventListener("submit", saveNote);
cancelButton.addEventListener("click", resetForm);
refreshButton.addEventListener("click", loadNotes);

setAuthMode("login");
renderSession();
