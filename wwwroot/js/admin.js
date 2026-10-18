const adminState = {
  token: localStorage.getItem("buzaAdminToken"),
  activeTab: "products",
  products: [],
  events: [],
  settings: null
};

const app = document.getElementById("adminApp");
const logoutButton = document.getElementById("logoutButton");

document.addEventListener("DOMContentLoaded", async () => {
  logoutButton.addEventListener("click", logout);
  renderAdmin();
});

async function renderAdmin() {
  logoutButton.hidden = !adminState.token;

  if (!adminState.token) {
    renderLogin();
    return;
  }

  app.innerHTML = `
    <div class="admin-tabs">
      ${tabButton("products", "Товары")}
      ${tabButton("events", "Мероприятия")}
      ${tabButton("settings", "Настройки")}
    </div>
    <div class="admin-status" id="adminStatus"></div>
    <section class="admin-layout" id="adminContent"></section>
  `;

  document.querySelectorAll("[data-admin-tab]").forEach((button) => {
    button.addEventListener("click", () => {
      adminState.activeTab = button.dataset.adminTab;
      renderAdmin();
    });
  });

  try {
    if (adminState.activeTab === "products") {
      await loadProducts();
      renderProducts();
    }

    if (adminState.activeTab === "events") {
      await loadEvents();
      renderEventPosts();
    }

    if (adminState.activeTab === "settings") {
      await loadSettings();
      renderSettings();
    }
  } catch (error) {
    setStatus(error.message, true);
  }
}

function renderLogin() {
  app.innerHTML = `
    <section class="admin-panel login-panel">
      <h1>Вход</h1>
      <form class="admin-form" id="loginForm" style="margin-top: 18px">
        <label>Логин
          <input name="login" autocomplete="username" required>
        </label>
        <label>Пароль
          <input name="password" type="password" autocomplete="current-password" required>
        </label>
        <button class="button primary" type="submit">Войти</button>
        <div class="form-result" id="loginResult"></div>
      </form>
    </section>
  `;

  document.getElementById("loginForm").addEventListener("submit", handleLogin);
}

async function handleLogin(event) {
  event.preventDefault();
  const form = event.target;
  const result = document.getElementById("loginResult");
  result.classList.remove("error");
  result.textContent = "Проверка...";

  try {
    const response = await publicApi("/api/auth/login", {
      method: "POST",
      body: JSON.stringify({
        login: form.elements.login.value,
        password: form.elements.password.value
      })
    });

    adminState.token = response.token;
    localStorage.setItem("buzaAdminToken", response.token);
    await renderAdmin();
  } catch (error) {
    result.classList.add("error");
    result.textContent = error.message;
  }
}

function logout() {
  localStorage.removeItem("buzaAdminToken");
  adminState.token = null;
  renderAdmin();
}

async function loadProducts() {
  adminState.products = await adminApi("/api/admin/products");
}

async function loadEvents() {
  adminState.events = await adminApi("/api/admin/events");
}

async function loadSettings() {
  adminState.settings = await publicApi("/api/settings");
}

function renderProducts() {
  const content = document.getElementById("adminContent");
  content.innerHTML = `
    <section class="admin-panel">
      <h2>Товар</h2>
      ${productForm()}
    </section>
    <section class="admin-panel">
      <h2>Список товаров</h2>
      ${adminState.products.length ? productsTable() : `<div class="empty-state">Товары еще не созданы.</div>`}
    </section>
  `;

  document.getElementById("productForm").addEventListener("submit", saveProduct);
  document.getElementById("resetProductForm").addEventListener("click", () => fillProductForm(null));
  content.querySelectorAll("[data-edit-product]").forEach((button) => {
    button.addEventListener("click", () => {
      const product = adminState.products.find((item) => item.id === Number(button.dataset.editProduct));
      fillProductForm(product);
      window.scrollTo({ top: 0, behavior: "smooth" });
    });
  });
  content.querySelectorAll("[data-delete-product]").forEach((button) => button.addEventListener("click", () => deleteProduct(Number(button.dataset.deleteProduct))));
  content.querySelectorAll("[data-delete-image]").forEach((button) => {
    button.addEventListener("click", () => deleteImage(Number(button.dataset.productId), Number(button.dataset.deleteImage)));
  });
}

function productForm(product = null) {
  return `
    <form class="admin-form" id="productForm" enctype="multipart/form-data">
      <input type="hidden" name="id" value="${product?.id || ""}">
      <div class="form-grid">
        <label>Название
          <input name="name" required maxlength="160" value="${escapeAttr(product?.name || "")}">
        </label>
        <label>Объем
          <input name="volume" required maxlength="80" value="${escapeAttr(product?.volume || "")}">
        </label>
      </div>
      <label>Краткое описание
        <textarea name="shortDescription" required maxlength="500">${escapeHtml(product?.shortDescription || "")}</textarea>
      </label>
      <label>Полное описание
        <textarea name="description" required>${escapeHtml(product?.description || "")}</textarea>
      </label>
      <label>Состав
        <textarea name="composition" required>${escapeHtml(product?.composition || "")}</textarea>
      </label>
      <div class="form-grid">
        <label>Цена
          <input name="price" type="number" min="0" step="0.01" required value="${product?.price || 0}">
        </label>
        <label>Изображение
          <input name="image" type="file" accept=".jpg,.jpeg,.png,.webp,.gif">
        </label>
      </div>
      <label class="check">
        <input name="isActive" type="checkbox" ${product?.isActive === false ? "" : "checked"}>
        <span>Опубликован</span>
      </label>
      <div class="actions">
        <button class="button primary" type="submit">Сохранить</button>
        <button class="button secondary" type="button" id="resetProductForm">Очистить</button>
      </div>
    </form>
  `;
}

function productsTable() {
  return `
    <table class="admin-table">
      <thead>
        <tr>
          <th>Товар</th>
          <th>Цена</th>
          <th>Статус</th>
          <th>Изображения</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        ${adminState.products.map((product) => `
          <tr>
            <td><strong>${escapeHtml(product.name)}</strong><br>${escapeHtml(product.shortDescription)}</td>
            <td>${formatPrice(product.price)}<br>${escapeHtml(product.volume)}</td>
            <td>${product.isActive ? "Опубликован" : "Скрыт"}</td>
            <td>${imageList(product)}</td>
            <td>
              <div class="row-actions">
                <button class="button secondary" type="button" data-edit-product="${product.id}">Изменить</button>
                <button class="button danger" type="button" data-delete-product="${product.id}">Удалить</button>
              </div>
            </td>
          </tr>
        `).join("")}
      </tbody>
    </table>
  `;
}

function imageList(product) {
  if (!product.images?.length) {
    return "Нет изображений";
  }

  return `
    <div class="image-list">
      ${product.images.map((image) => `
        <div class="image-item">
          <img src="${escapeAttr(image.filePath)}" alt="">
          <button class="button secondary" type="button" data-product-id="${product.id}" data-delete-image="${image.id}">Удалить</button>
        </div>
      `).join("")}
    </div>
  `;
}

function fillProductForm(product) {
  const wrapper = document.querySelector(".admin-panel");
  wrapper.innerHTML = `<h2>Товар</h2>${productForm(product)}`;
  document.getElementById("productForm").addEventListener("submit", saveProduct);
  document.getElementById("resetProductForm").addEventListener("click", () => fillProductForm(null));
}

async function saveProduct(event) {
  event.preventDefault();
  const form = event.target;
  const id = form.elements.id.value;
  const payload = {
    name: form.elements.name.value,
    shortDescription: form.elements.shortDescription.value,
    description: form.elements.description.value,
    composition: form.elements.composition.value,
    volume: form.elements.volume.value,
    price: Number(form.elements.price.value),
    isActive: form.elements.isActive.checked
  };

  try {
    const product = await adminApi(id ? `/api/admin/products/${id}` : "/api/admin/products", {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(payload)
    });

    const image = form.elements.image.files[0];
    if (image) {
      const data = new FormData();
      data.append("file", image);
      await adminApi(`/api/admin/products/${product.id}/images`, {
        method: "POST",
        body: data
      });
    }

    setStatus("Товар сохранен.");
    await loadProducts();
    renderProducts();
  } catch (error) {
    setStatus(error.message, true);
  }
}

async function deleteProduct(id) {
  if (!confirm("Удалить товар?")) {
    return;
  }

  try {
    await adminApi(`/api/admin/products/${id}`, { method: "DELETE" });
    setStatus("Товар удален.");
    await loadProducts();
    renderProducts();
  } catch (error) {
    setStatus(error.message, true);
  }
}

async function deleteImage(productId, imageId) {
  try {
    await adminApi(`/api/admin/products/${productId}/images/${imageId}`, { method: "DELETE" });
    setStatus("Изображение удалено.");
    await loadProducts();
    renderProducts();
  } catch (error) {
    setStatus(error.message, true);
  }
}

function renderEventPosts() {
  const content = document.getElementById("adminContent");
  content.innerHTML = `
    <section class="admin-panel">
      <h2>Публикация о мероприятии</h2>
      <div id="eventFormContainer">${eventPostForm()}</div>
    </section>
    <section class="admin-panel">
      <h2>Список публикаций</h2>
      ${adminState.events.length ? eventPostsTable() : `<div class="empty-state">Мероприятия еще не добавлены.</div>`}
    </section>
  `;

  bindEventPostForm();
  content.querySelectorAll("[data-edit-event]").forEach((button) => {
    button.addEventListener("click", () => {
      const post = adminState.events.find((item) => item.id === Number(button.dataset.editEvent));
      fillEventPostForm(post);
      window.scrollTo({ top: 0, behavior: "smooth" });
    });
  });
  content.querySelectorAll("[data-delete-event]").forEach((button) => {
    button.addEventListener("click", () => deleteEventPost(Number(button.dataset.deleteEvent)));
  });
  content.querySelectorAll("[data-delete-event-image]").forEach((button) => {
    button.addEventListener("click", () => deleteEventImage(Number(button.dataset.eventId), Number(button.dataset.deleteEventImage)));
  });
}

function eventPostForm(post = null) {
  return `
    <form class="admin-form" id="eventPostForm" enctype="multipart/form-data">
      <input type="hidden" name="id" value="${post?.id || ""}">
      <label>Заголовок
        <input name="title" required maxlength="200" value="${escapeAttr(post?.title || "")}">
      </label>
      <label>Общий текст поста
        <textarea name="content" required>${escapeHtml(post?.content || "")}</textarea>
      </label>
      <label>Картинки
        <input name="images" type="file" accept=".jpg,.jpeg,.png,.webp,.gif" multiple>
        <small>Можно выбрать до 10 файлов за раз, до 5 МБ каждый.</small>
      </label>
      ${post?.images?.length ? `<div><strong>Загруженные картинки</strong>${eventImageList(post)}</div>` : ""}
      <label class="check">
        <input name="isPublished" type="checkbox" ${post?.isPublished === false ? "" : "checked"}>
        <span>Опубликовано на сайте</span>
      </label>
      <div class="actions">
        <button class="button primary" type="submit">Сохранить</button>
        <button class="button secondary" type="button" id="resetEventPostForm">Новая публикация</button>
      </div>
    </form>
  `;
}

function bindEventPostForm() {
  document.getElementById("eventPostForm").addEventListener("submit", saveEventPost);
  document.getElementById("resetEventPostForm").addEventListener("click", () => fillEventPostForm(null));
}

function eventPostsTable() {
  return `
    <table class="admin-table">
      <thead><tr><th>Мероприятие</th><th>Дата</th><th>Статус</th><th>Фото</th><th>Действия</th></tr></thead>
      <tbody>
        ${adminState.events.map((post) => `
          <tr>
            <td><strong>${escapeHtml(post.title)}</strong><br>${escapeHtml(post.content.slice(0, 180))}${post.content.length > 180 ? "…" : ""}</td>
            <td>${new Date(post.createdAt).toLocaleDateString("ru-RU")}</td>
            <td>${post.isPublished ? "Опубликовано" : "Скрыто"}</td>
            <td>${post.images?.length || 0}</td>
            <td><div class="row-actions">
              <button class="button secondary" type="button" data-edit-event="${post.id}">Изменить</button>
              <button class="button danger" type="button" data-delete-event="${post.id}">Удалить</button>
            </div></td>
          </tr>
        `).join("")}
      </tbody>
    </table>
  `;
}

function eventImageList(post) {
  return `
    <div class="image-list" style="margin-top: 12px">
      ${post.images.map((image) => `
        <div class="image-item">
          <img src="${escapeAttr(image.filePath)}" alt="">
          <button class="button secondary" type="button" data-event-id="${post.id}" data-delete-event-image="${image.id}">Удалить фото</button>
        </div>
      `).join("")}
    </div>
  `;
}

function fillEventPostForm(post) {
  document.getElementById("eventFormContainer").innerHTML = eventPostForm(post);
  bindEventPostForm();
}

async function saveEventPost(event) {
  event.preventDefault();
  const form = event.target;
  const id = form.elements.id.value;
  const payload = {
    title: form.elements.title.value,
    content: form.elements.content.value,
    isPublished: form.elements.isPublished.checked
  };

  try {
    const post = await adminApi(id ? `/api/admin/events/${id}` : "/api/admin/events", {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(payload)
    });
    form.elements.id.value = post.id;
    const images = Array.from(form.elements.images.files);
    if (images.length) {
      if (images.length > 10) {
        throw new Error("За одну загрузку можно выбрать не больше 10 картинок.");
      }
      const data = new FormData();
      images.forEach((image) => data.append("files", image));
      await adminApi(`/api/admin/events/${post.id}/images`, { method: "POST", body: data });
    }

    setStatus("Публикация сохранена.");
    await loadEvents();
    renderEventPosts();
  } catch (error) {
    setStatus(error.message, true);
  }
}

async function deleteEventPost(id) {
  if (!confirm("Удалить публикацию и все ее картинки?")) {
    return;
  }

  try {
    await adminApi(`/api/admin/events/${id}`, { method: "DELETE" });
    setStatus("Публикация удалена.");
    await loadEvents();
    renderEventPosts();
  } catch (error) {
    setStatus(error.message, true);
  }
}

async function deleteEventImage(eventId, imageId) {
  try {
    await adminApi(`/api/admin/events/${eventId}/images/${imageId}`, { method: "DELETE" });
    setStatus("Картинка удалена.");
    await loadEvents();
    renderEventPosts();
  } catch (error) {
    setStatus(error.message, true);
  }
}

function renderSettings() {
  const content = document.getElementById("adminContent");
  const s = adminState.settings;
  content.innerHTML = `
    <section class="admin-panel">
      <h2>Настройки сайта</h2>
      <form class="admin-form" id="settingsForm">
        ${settingsInput("projectName", "Название проекта", s.projectName, true)}
        ${settingsInput("logoPath", "Путь к логотипу", s.logoPath)}
        ${settingsInput("mainImagePath", "Главное изображение", s.mainImagePath)}
        <div class="form-grid">
          ${settingsInput("phone", "Телефон", s.phone)}
          ${settingsInput("email", "Email", s.email)}
        </div>
        ${settingsInput("address", "Адрес", s.address)}
        ${settingsTextarea("foundationText", "Текст поддержки", s.foundationText)}
        <div class="form-grid">
          ${settingsInput("foundationUrl", "Ссылка фонда", s.foundationUrl)}
          ${settingsInput("federalProjectUrl", "Ссылка федерального проекта", s.federalProjectUrl)}
        </div>
        ${settingsInput("heroTitle", "Заголовок первого экрана", s.heroTitle)}
        ${settingsTextarea("heroDescription", "Описание первого экрана", s.heroDescription)}
        ${settingsTextarea("productHistory", "История продукта", s.productHistory)}
        ${settingsTextarea("productDescription", "Описание продукта", s.productDescription)}
        ${settingsTextarea("productComposition", "Состав", s.productComposition)}
        ${settingsTextarea("productBenefits", "Польза продукта", s.productBenefits)}
        ${settingsTextarea("technologyText", "Описание технологии", s.technologyText)}
        ${settingsTextarea("productionSteps", "Этапы изготовления", s.productionSteps)}
        ${settingsTextarea("rawMaterials", "Используемое сырье", s.rawMaterials)}
        ${settingsTextarea("startupDescription", "Описание стартапа", s.startupDescription)}
        ${settingsTextarea("projectGoals", "Цели проекта", s.projectGoals)}
        ${settingsTextarea("developmentStages", "Этапы развития", s.developmentStages)}
        ${settingsTextarea("teamInfo", "Команда", s.teamInfo)}
        ${settingsTextarea("developmentPlans", "Планы развития", s.developmentPlans)}
        <button class="button primary" type="submit">Сохранить настройки</button>
      </form>
    </section>
  `;

  document.getElementById("settingsForm").addEventListener("submit", saveSettings);
}

function settingsInput(name, label, value, required = false) {
  return `
    <label>${escapeHtml(label)}
      <input name="${name}" value="${escapeAttr(value || "")}" ${required ? "required" : ""}>
    </label>
  `;
}

function settingsTextarea(name, label, value) {
  return `
    <label>${escapeHtml(label)}
      <textarea name="${name}">${escapeHtml(value || "")}</textarea>
    </label>
  `;
}

async function saveSettings(event) {
  event.preventDefault();
  const form = event.target;
  const payload = {};
  Array.from(form.elements).forEach((element) => {
    if (element.name) {
      payload[element.name] = element.value;
    }
  });

  try {
    adminState.settings = await adminApi("/api/admin/settings", {
      method: "PUT",
      body: JSON.stringify(payload)
    });
    setStatus("Настройки сохранены.");
  } catch (error) {
    setStatus(error.message, true);
  }
}

function tabButton(name, label) {
  return `<button type="button" class="${adminState.activeTab === name ? "active" : ""}" data-admin-tab="${name}">${label}</button>`;
}

async function publicApi(url, options = {}) {
  return request(url, options);
}

async function adminApi(url, options = {}) {
  return request(url, {
    ...options,
    headers: {
      Authorization: `Bearer ${adminState.token}`,
      ...(options.headers || {})
    }
  });
}

async function request(url, options = {}) {
  const isFormData = options.body instanceof FormData;
  const response = await fetch(url, {
    ...options,
    headers: {
      ...(isFormData ? {} : { "Content-Type": "application/json" }),
      ...(options.headers || {})
    }
  });

  if (response.status === 401) {
    localStorage.removeItem("buzaAdminToken");
    adminState.token = null;
  }

  if (!response.ok) {
    throw new Error(await readError(response));
  }

  return response.status === 204 ? null : response.json();
}

async function readError(response) {
  try {
    const data = await response.json();
    if (data.error) {
      return data.error;
    }

    if (data.errors) {
      return Object.values(data.errors).flat().join(" ");
    }
  } catch {
    return "Ошибка запроса.";
  }

  return "Ошибка запроса.";
}

function setStatus(message, isError = false) {
  const status = document.getElementById("adminStatus");
  if (!status) {
    return;
  }

  status.textContent = message;
  status.classList.toggle("error", isError);
}

function formatPrice(value) {
  return new Intl.NumberFormat("ru-RU", {
    style: "currency",
    currency: "RUB",
    maximumFractionDigits: 0
  }).format(value);
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function escapeAttr(value) {
  return escapeHtml(value).replaceAll("`", "&#096;");
}
