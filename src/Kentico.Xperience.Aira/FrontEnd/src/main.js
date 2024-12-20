import ChatComponent from "./Chat.vue";
import AssetsComponent from "./Assets.vue";
import { createApp } from "vue";

const chatElement = document.getElementById("chat-app");
const assetsElement = document.getElementById("assets-app");

if (chatElement) {
    const pathsModel = JSON.parse(chatElement.dataset.pathsModel || "{}");
    const baseUrl = chatElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(chatElement.dataset.navBarModel || "{}");
    const history = JSON.parse(chatElement.dataset.history || {});
    const initialAiraMessage = chatElement.dataset.initialAiraMessage;

    createApp(ChatComponent, {
        pathsModel,
        baseUrl,
        navBarModel,
        history,
        initialAiraMessage
    }).mount("#chat-app");
}

if (assetsElement) {
    const pathsModel = JSON.parse(assetsElement.dataset.pathsModel || "{}");
    const baseUrl = assetsElement.dataset.baseUrl || "";
    const navBarModel = JSON.parse(assetsElement.dataset.navBarModel || "{}");
    
    createApp(AssetsComponent, {
        pathsModel,
        baseUrl,
        navBarModel
    }).mount("#assets-app");
}