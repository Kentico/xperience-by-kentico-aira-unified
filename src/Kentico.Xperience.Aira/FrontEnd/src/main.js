import ChatComponent from "./Chat.vue";
import AssetsComponent from "./Assets.vue";
import { createApp } from "vue";

const chatElement = document.getElementById("chat-app");
const assetsElement = document.getElementById("assets-app");
const signinElement = document.getElementById("aira-kentico-admin-signin");

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

function verifyAuthentication() {
    fetch('/check')
        .then(response => {
            if (response.ok) {
                window.location.href = '/aira/chat';
            }
        })
        .catch(error => {
            console.error(error);
        });
}


if (signinElement) {
    document.addEventListener('DOMContentLoaded', () => {
        const loginButton = document.getElementById('loginButton');
        if (loginButton) {
            loginButton.addEventListener('click', () => {
                const adminLoginUrl = '/admin';
                const popup = window.open(adminLoginUrl, 'Login', 'width=600,height=800');
    
                const pollTimer = setInterval(() => {
                    if (popup.closed) {
                        clearInterval(pollTimer);
                        verifyAuthentication();
                    }
                }, 1000);
            });
        } else {
            console.error('#loginButton not found in the DOM.');
        }
    });
}