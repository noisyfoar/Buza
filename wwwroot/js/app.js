const state = {
  settings: null,
  products: [],
  events: []
};

const fallbackImage = "/assets/buza-product.png";
const processImage = "/assets/buza-process.png";

document.addEventListener("DOMContentLoaded", async () => {
  bindNavigation();
  await loadInitialData();
  render();
});

window.addEventListener("hashchange", render);

async function loadInitialData() {
  try {
    const [settings, products] = await Promise.all([
      api("/api/settings"),
      api("/api/products")
    ]);

    state.settings = settings;
    state.products = products;
    applyChrome(settings);
  } catch (error) {
    document.getElementById("app").innerHTML = `<section class="section"><div class="section-inner empty-state">${escapeHtml(error.message)}</div></section>`;
  }
}

function bindNavigation() {
  const toggle = document.querySelector("[data-nav-toggle]");
  const nav = document.getElementById("mainNav");

  toggle.addEventListener("click", () => {
    const isOpen = nav.classList.toggle("open");
    toggle.setAttribute("aria-expanded", String(isOpen));
  });

  nav.addEventListener("click", () => {
    nav.classList.remove("open");
    toggle.setAttribute("aria-expanded", "false");
  });

}

function applyChrome(settings) {
  document.title = settings.projectName;
  document.getElementById("brandName").textContent = settings.projectName;
  document.getElementById("footerName").textContent = settings.projectName;

  const logo = document.getElementById("brandLogo");
  const mark = document.getElementById("brandMark");
  if (settings.logoPath) {
    logo.src = settings.logoPath;
    logo.hidden = false;
    mark.hidden = true;
  }
}

function getRoute() {
  const hash = window.location.hash.replace(/^#\/?/, "");
  const [path = "", query = ""] = hash.split("?");
  return {
    path: path || "",
    params: new URLSearchParams(query)
  };
}

async function render() {
  if (!state.settings) {
    return;
  }

  const route = getRoute();
  setActiveNav(route.path);

  if (route.path === "" || route.path === "/") {
    renderHome();
    return;
  }

  if (route.path === "product") {
    renderProductInfo();
    return;
  }

  if (route.path === "catalog") {
    renderCatalog();
    return;
  }

  if (route.path === "events") {
    await renderEvents();
    return;
  }

  if (route.path.startsWith("product/")) {
    await renderProductDetails(Number(route.path.split("/")[1]));
    return;
  }

  if (route.path === "technology") {
    renderTechnology();
    return;
  }

  if (route.path === "project") {
    renderProject();
    return;
  }

  if (route.path === "contacts") {
    renderContacts();
    return;
  }

  renderNotFound();
}

function setActiveNav(path) {
  document.querySelectorAll(".main-nav a").forEach((link) => {
    const target = link.getAttribute("href").replace(/^#\/?/, "");
    link.classList.toggle("active", target === path || (target === "" && path === ""));
  });
}

function renderHome() {
  const s = state.settings;
  const heroImage = s.mainImagePath || "/assets/buza-hero.png";
  document.getElementById("app").innerHTML = `
    <section class="hero" style="background-image: url('${escapeAttr(heroImage)}')">
      <div class="hero-content">
        <p class="eyebrow">Башкирская традиция в современном формате</p>
        <h1>${escapeHtml(s.heroTitle)}</h1>
        <p class="lead">${escapeHtml(s.heroDescription)}</p>
        <div class="hero-actions">
          <a class="button primary" href="#/catalog">Перейти в каталог</a>
          <a class="button ghost" href="#/contacts">Связаться</a>
        </div>
      </div>
    </section>
    <section class="section">
      <div class="section-inner">
        <p class="eyebrow">Преимущества</p>
        <h2>Почему буза интересна покупателю</h2>
        <div class="grid three" style="margin-top: 26px">
          ${advantageCard("01", "Натуральная основа", "Зерновой ферментированный напиток с мягким вкусом и понятным составом.")}
          ${advantageCard("02", "Национальная идентичность", "Продукт знакомит с башкирской гастрономической культурой без музейной дистанции.")}
          ${advantageCard("03", "Гибкий формат", "Каталог можно расширять новыми объемами, вкусами и сезонными партиями.")}
        </div>
      </div>
    </section>
    <section class="support-band section">
      <div class="section-inner">
        <p class="eyebrow">Поддержка проекта</p>
        <h2>Грантовое развитие стартапа</h2>
        <p class="lead">${escapeHtml(s.foundationText)}</p>
        <div class="support-links">
          ${s.foundationUrl ? `<a href="${escapeAttr(s.foundationUrl)}" target="_blank" rel="noreferrer">Фонд поддержки</a>` : ""}
          ${s.federalProjectUrl ? `<a href="${escapeAttr(s.federalProjectUrl)}" target="_blank" rel="noreferrer">Федеральный проект</a>` : ""}
        </div>
      </div>
    </section>
  `;
}

function renderProductInfo() {
  const s = state.settings;
  document.getElementById("app").innerHTML = `
    ${pageIntro("О продукте", "Буза как национальный напиток", s.productDescription)}
    <section class="section compact">
      <div class="section-inner text-columns">
        <div class="media-frame"><img src="${escapeAttr(s.mainImagePath || "/assets/buza-hero.png")}" alt="Буза"></div>
        <div class="grid">
          ${infoPanel("История продукта", s.productHistory)}
          ${infoPanel("Состав", s.productComposition)}
          ${infoPanel("Польза продукта", s.productBenefits)}
        </div>
      </div>
    </section>
    <section class="section compact">
      <div class="section-inner">
        <p class="eyebrow">Галерея</p>
        <h2>Визуальные материалы продукта</h2>
        <div class="gallery" style="margin-top: 22px">
          <img src="${escapeAttr(s.mainImagePath || "/assets/buza-hero.png")}" alt="Буза в бутылке">
          <img src="${fallbackImage}" alt="Каталожное изображение бузы">
          <img src="${processImage}" alt="Сырье и технология бузы">
        </div>
      </div>
    </section>
  `;
}

function renderCatalog() {
  const products = state.products;
  document.getElementById("app").innerHTML = `
    ${pageIntro("Каталог", "Продукция стартапа", "Актуальные позиции загружаются из базы данных. Администратор может добавлять, скрывать и обновлять товары.")}
    <section class="section compact">
      <div class="section-inner">
        ${products.length ? `<div class="grid three">${products.map(productCard).join("")}</div>` : `<div class="empty-state">В каталоге пока нет опубликованных товаров.</div>`}
      </div>
    </section>
  `;
}

async function renderEvents() {
  const app = document.getElementById("app");
  app.innerHTML = `<section class="section"><div class="section-inner empty-state">Загрузка мероприятий...</div></section>`;

  try {
    state.events = await api("/api/events");
    app.innerHTML = `
      ${pageIntro("Мероприятия", "События проекта", "Новости, встречи и события команды «Бузы».")}
      <section class="section compact">
        <div class="section-inner grid">
          ${state.events.length ? state.events.map(eventPostCard).join("") : `<div class="empty-state">Пока нет опубликованных мероприятий.</div>`}
        </div>
      </section>
    `;
  } catch (error) {
    app.innerHTML = `<section class="section"><div class="section-inner empty-state">${escapeHtml(error.message)}</div></section>`;
  }
}

function eventPostCard(post) {
  return `
    <article class="event-post">
      <div class="event-post-header">
        <h2>${escapeHtml(post.title)}</h2>
        <time datetime="${escapeAttr(post.createdAt)}">${new Date(post.createdAt).toLocaleDateString("ru-RU")}</time>
      </div>
      ${post.images?.length ? `<div class="event-gallery">${post.images.map((image, index) => `<img src="${escapeAttr(image.filePath)}" alt="${escapeAttr(post.title)} — фото ${index + 1}" loading="lazy">`).join("")}</div>` : ""}
      <p class="rich-text event-post-content">${escapeHtml(post.content)}</p>
    </article>
  `;
}

async function renderProductDetails(id) {
  const app = document.getElementById("app");
  app.innerHTML = `<section class="section"><div class="section-inner empty-state">Загрузка товара...</div></section>`;

  try {
    const product = await api(`/api/products/${id}`);
    const firstImage = product.images?.[0]?.filePath || fallbackImage;
    app.innerHTML = `
      ${pageIntro("Страница товара", product.name, product.shortDescription)}
      <section class="section compact">
        <div class="section-inner text-columns">
          <div>
            <div class="media-frame"><img src="${escapeAttr(firstImage)}" alt="${escapeAttr(product.name)}"></div>
            <div class="gallery" style="margin-top: 14px">
              ${(product.images || []).map((image) => `<img src="${escapeAttr(image.filePath)}" alt="${escapeAttr(product.name)}">`).join("")}
            </div>
          </div>
          <div class="grid">
            ${infoPanel("Описание", product.description)}
            ${infoPanel("Состав", product.composition)}
            <div class="info-panel">
              <h3>Характеристики</h3>
              <p><strong>Объем:</strong> ${escapeHtml(product.volume)}</p>
              <p class="price">${formatPrice(product.price)}</p>
            </div>
          </div>
        </div>
      </section>
    `;
  } catch (error) {
    app.innerHTML = `<section class="section"><div class="section-inner empty-state">${escapeHtml(error.message)}</div></section>`;
  }
}

function renderTechnology() {
  const s = state.settings;
  document.getElementById("app").innerHTML = `
    ${pageIntro("Технология", "Производство бузы", s.technologyText)}
    <section class="section compact">
      <div class="section-inner text-columns">
        <div class="grid">
          ${infoPanel("Этапы изготовления", s.productionSteps)}
          ${infoPanel("Используемое сырье", s.rawMaterials)}
          ${infoPanel("Особенности технологии", "Контроль температуры, времени ферментации и чистоты оборудования помогает получать стабильный вкус и безопасный продукт.")}
        </div>
        <div class="media-frame"><img src="${processImage}" alt="Процесс производства бузы"></div>
      </div>
    </section>
  `;
}

function renderProject() {
  const s = state.settings;
  document.getElementById("app").innerHTML = `
    ${pageIntro("О проекте", "Стартап национального напитка", s.startupDescription)}
    <section class="section compact">
      <div class="section-inner grid two">
        ${infoPanel("Цели проекта", s.projectGoals)}
        ${infoPanel("Этапы развития", s.developmentStages)}
        ${infoPanel("Команда", s.teamInfo)}
        ${infoPanel("Планы развития", s.developmentPlans)}
      </div>
    </section>
  `;
}

function renderContacts() {
  const s = state.settings;
  document.getElementById("app").innerHTML = `
    ${pageIntro("Контакты", "Связаться с проектом", "Уточните наличие продукции или предложите сотрудничество по телефону или электронной почте.")}
    <section class="section compact">
      <div class="section-inner text-columns">
        <div class="grid">
          <div class="info-panel">
            <h3>Контактная информация</h3>
            <p><strong>Телефон:</strong> ${escapeHtml(s.phone)}</p>
            <p><strong>Email:</strong> <a href="mailto:${escapeAttr(s.email)}">${escapeHtml(s.email)}</a></p>
            <p><strong>Адрес:</strong> ${escapeHtml(s.address)}</p>
          </div>
        </div>
      </div>
    </section>
  `;
}

function renderNotFound() {
  document.getElementById("app").innerHTML = `
    <section class="section">
      <div class="section-inner empty-state">Страница не найдена.</div>
    </section>
  `;
}

function pageIntro(eyebrow, title, text) {
  return `
    <section class="section">
      <div class="section-inner">
        <p class="eyebrow">${escapeHtml(eyebrow)}</p>
        <h1>${escapeHtml(title)}</h1>
        <p class="lead rich-text">${escapeHtml(text)}</p>
      </div>
    </section>
  `;
}

function advantageCard(icon, title, text) {
  return `
    <article class="card">
      <span class="advantage-icon">${escapeHtml(icon)}</span>
      <h3>${escapeHtml(title)}</h3>
      <p>${escapeHtml(text)}</p>
    </article>
  `;
}

function infoPanel(title, text) {
  return `
    <article class="info-panel">
      <h3>${escapeHtml(title)}</h3>
      <p class="rich-text">${escapeHtml(text)}</p>
    </article>
  `;
}

function productCard(product) {
  return `
    <article class="product-card">
      <img src="${escapeAttr(product.imagePath || fallbackImage)}" alt="${escapeAttr(product.name)}">
      <div class="product-body">
        <h3>${escapeHtml(product.name)}</h3>
        <p>${escapeHtml(product.shortDescription)}</p>
        <div class="product-meta">
          <span>${escapeHtml(product.volume)}</span>
          <span class="price">${formatPrice(product.price)}</span>
        </div>
        <div class="actions">
          <a class="button secondary" href="#/product/${product.id}">Подробнее</a>
        </div>
      </div>
    </article>
  `;
}

async function api(url, options = {}) {
  const response = await fetch(url, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(options.headers || {})
    }
  });

  if (!response.ok) {
    const error = await readError(response);
    throw new Error(error);
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

function parseSocialLinks(raw) {
  return String(raw || "")
    .split("\n")
    .map((line) => line.trim())
    .filter(Boolean)
    .map((line) => {
      const [label, ...urlParts] = line.split(":");
      const href = urlParts.join(":").trim();
      return href ? { label: label.trim(), href } : { label: line, href: line };
    });
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
