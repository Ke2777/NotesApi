const apiUrl = "/notes";

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

let notes = [];
let editingId = null;

function setStatus(message, isError = false) {
    statusText.textContent = message;
    statusText.classList.toggle("error", isError);
}

function resetForm() {
    editingId = null;
    form.reset();
    submitButton.textContent = "Создать";
    cancelButton.hidden = true;
    setStatus("");
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

async function loadNotes() {
    setStatus("Загрузка...");

    try {
        const response = await fetch(apiUrl);

        if (!response.ok) {
            throw new Error(`GET /notes returned ${response.status}`);
        }

        notes = await response.json();
        renderNotes();
        setStatus("");
    } catch (error) {
        setStatus("Не удалось загрузить заметки. Проверь API и базу данных.", true);
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
        setStatus("Заголовок должен быть минимум 3 символа.", true);
        return;
    }

    const url = editingId === null ? apiUrl : `${apiUrl}/${editingId}`;
    const method = editingId === null ? "POST" : "PUT";

    try {
        const response = await fetch(url, {
            method,
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error(`${method} ${url} returned ${response.status}`);
        }

        resetForm();
        await loadNotes();
    } catch (error) {
        setStatus("Не удалось сохранить заметку.", true);
        console.error(error);
    }
}

async function deleteNote(id) {
    try {
        const response = await fetch(`${apiUrl}/${id}`, { method: "DELETE" });

        if (!response.ok) {
            throw new Error(`DELETE /notes/${id} returned ${response.status}`);
        }

        if (editingId === id) {
            resetForm();
        }

        await loadNotes();
    } catch (error) {
        setStatus("Не удалось удалить заметку.", true);
        console.error(error);
    }
}

form.addEventListener("submit", saveNote);
cancelButton.addEventListener("click", resetForm);
refreshButton.addEventListener("click", loadNotes);

loadNotes();
