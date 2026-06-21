import { expect, test } from "@playwright/test";

const onePixelPng = Buffer.from(
  "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/p9sAAAAASUVORK5CYII=",
  "base64"
);

test("public navigation switches all required pages", async ({ page }) => {
  await page.goto("/");
  await expect(page.getByRole("heading", { name: /Натуральная буза/ })).toBeVisible();

  const pages = [
    ["О продукте", /#\/product$/, "Буза как национальный напиток"],
    ["Каталог", /#\/catalog$/, "Продукция стартапа"],
    ["Технология", /#\/technology$/, "Производство бузы"],
    ["О проекте", /#\/project$/, "Стартап национального напитка"],
    ["Контакты", /#\/contacts$/, "Связаться с проектом"]
  ];

  for (const [linkName, urlPattern, heading] of pages) {
    await page.getByRole("link", { name: linkName, exact: true }).click();
    await expect(page).toHaveURL(urlPattern);
    await expect(page.getByRole("heading", { name: heading })).toBeVisible();
  }
});

test("catalog details and request buttons navigate to product and feedback form", async ({ page }) => {
  await page.goto("/#/catalog");

  await page.getByRole("link", { name: "Подробнее" }).first().click();
  await expect(page).toHaveURL(/#\/product\/\d+$/);
  await expect(page.locator("main h1")).toContainText("Буза");

  await page.getByRole("button", { name: "Оставить заявку" }).click();
  await expect(page).toHaveURL(/#\/contacts\?product=\d+$/);
  await expect(page.locator("select[name='productId']")).not.toHaveValue("");
});

test("feedback form button creates a request and shows success message", async ({ page }) => {
  await page.goto("/#/contacts");

  await page.locator("input[name='name']").fill("E2E покупатель");
  await page.locator("input[name='phone']").fill("+7 999 123-45-67");
  await page.locator("input[name='email']").fill("e2e-buyer@example.com");
  await page.locator("textarea[name='message']").fill("Проверка создания заявки через кнопку формы");
  await page.locator("input[name='personalDataAccepted']").check();
  await page.getByRole("button", { name: "Отправить заявку" }).click();

  await expect(page.locator("[data-form-result]")).toContainText("Спасибо");
});

test("admin panel creates product, uploads image, and deletes product", async ({ page }) => {
  const productName = `Буза E2E ${Date.now()}`;

  await page.goto("/admin.html");
  await page.getByLabel("Логин").fill("admin");
  await page.getByLabel("Пароль").fill("admin123");
  await page.getByRole("button", { name: "Войти" }).click();

  await expect(page.getByRole("button", { name: "Товары" })).toHaveClass(/active/);
  await expect(page.getByRole("heading", { name: "Список товаров" })).toBeVisible();

  await page.locator("input[name='name']").fill(productName);
  await page.locator("input[name='volume']").fill("0,45 л");
  await page.locator("textarea[name='shortDescription']").fill("Товар создан Playwright-тестом");
  await page.locator("textarea[name='description']").fill("Полное описание товара для E2E проверки");
  await page.locator("textarea[name='composition']").fill("Овес, вода, закваска");
  await page.locator("input[name='price']").fill("333");
  await page.locator("input[name='image']").setInputFiles({
    name: "e2e-product.png",
    mimeType: "image/png",
    buffer: onePixelPng
  });
  await page.getByRole("button", { name: "Сохранить" }).click();

  await expect(page.locator("#adminStatus")).toContainText("Товар сохранен");
  const row = page.locator("tr").filter({ hasText: productName });
  await expect(row).toBeVisible();
  await expect(row.locator("img[src^='/uploads/']")).toBeVisible();

  page.once("dialog", dialog => dialog.accept());
  await row.locator("[data-delete-product]").click();

  await expect(page.locator("tr").filter({ hasText: productName })).toHaveCount(0);
});
