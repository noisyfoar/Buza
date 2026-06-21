const adminState = {
  token: localStorage.getItem("buzaAdminToken"),
  activeTab: "products",
  products: [],
  feedback: [],
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
      ${tabButton("feedback", "Заявки")}
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

    if (adminState.activeTab === "feedback") {
      await loadFeedback();
      renderFeedback();
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

async function loadFeedback() {
  adminState.feedback = await adminApi("/api/admin/feedback");
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

function renderFeedback() {
  const content = document.getElementById("adminContent");
  content.innerHTML = `
    <section class="admin-panel">
      <h2>Заявки</h2>
      ${adminState.feedback.length ? feedbackTable() : `<div class="empty-state">Заявок пока нет.</div>`}
    </section>
  `;

  content.querySelectorAll("[data-feedback-status]").forEach((select) => {
    select.addEventListener("change", () => updateFeedbackStatus(Number(select.dataset.feedbackStatus), select.value));
  });
}

function feedbackTable() {
  return `
    <table class="admin-table">
      <thead>
        <tr>
          <th>Контакт</th>
          <th>Товар</th>
          <th>Сообщение</th>
          <th>Статус</th>
        </tr>
      </thead>
      <tbody>
        ${adminState.feedback.map((item) => `
          <tr>
            <td>
              <strong>${escapeHtml(item.name)}</strong><br>
              ${escapeHtml(item.phone)}<br>
              <a href="mailto:${escapeAttr(item.email)}">${escapeHtml(item.email)}</a><br>
              ${new Date(item.createdAt).toLocaleString("ru-RU")}
            </td>
            <td>${escapeHtml(item.productName || "Не выбран")}</td>
            <td>${escapeHtml(item.message)}</td>
            <td>
              <select data-feedback-status="${item.id}">
                ${["NEW", "IN_PROGRESS", "COMPLETED", "CANCELED"].map((status) => `<option value="${status}" ${item.status === status ? "selected" : ""}>${status}</option>`).join("")}
              </select>
            </td>
          </tr>
        `).join("")}
      </tbody>
    </table>
  `;
}

async function updateFeedbackStatus(id, status) {
  try {
    await adminApi(`/api/admin/feedback/${id}`, {
      method: "PUT",
      body: JSON.stringify({ status })
    });
    setStatus("Статус заявки обновлен.");
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
        ${settingsTextarea("socialLinks", "Социальные сети", s.socialLinks)}
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
        ${settingsInput("formSuccessMessage", "Сообщение после отправки формы", s.formSuccessMessage)}
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
