import ChatComponent from "./Chat.vue";
import AssetsComponent from "./Assets.vue";
import { createApp } from "vue";

const chatElement = document.getElementById("chat-app");
const assetsElement = document.getElementById("assets-app");

if (chatElement) {
    const airaBaseUrl = chatElement.dataset.airaBaseUrl;
    const baseUrl = chatElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(chatElement.dataset.navBarModel || "{}");
    const history = JSON.parse(chatElement.dataset.history || {});
    const aiIconUrl = chatElement.dataset.aiIconUrl || "";

    createApp(ChatComponent, {
        airaBaseUrl,
        aiIconUrl,
        baseUrl,
        navBarModel,
        history
    }).mount("#chat-app");
}

if (assetsElement) {
    const airaBaseUrl = assetsElement.dataset.airaBaseUrl;
    const baseUrl = assetsElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(assetsElement.dataset.navBarModel || "{}");
    
    createApp(AssetsComponent, {
        airaBaseUrl,
        baseUrl,
        navBarModel
    }).mount("#assets-app");
}