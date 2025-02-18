<template>
    <div class="c-app_inner">
        <div id="loading" class="c-loading">
            <img :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/spinner.svg`" class="c-loading_spinner" alt="loading spinner" />
        </div>

        <div class="c-app_header">
            <NavBarComponent :airaBaseUrl="airaBaseUrl" :navBarModel="navBarModel" :baseUrl="baseUrl"/>
        </div>

        <div class="c-app_body">
            <div class="container">
                <deep-chat
                    :avatars="{
                        ai : { 
                            src: `${this.baseUrl}${this.aiIconUrl}`,
                            styles: {
                                avatar:
                                {
                                    height: '3rem',
                                    width: '3rem'
                                }
                            }
                        },
                        user : {
                            styles: {
                                avatar:
                                {
                                    height: '3rem',
                                    width: '3rem'
                                }
                            }
                        }
                    }"
                    :dropupStyles="{
                        button: {
                            styles: {
                                container: {
                                    default: { backgroundColor: '#eff8ff'},
                                    hover: { backgroundColor: '#e4f3ff'},
                                    click: { backgroundColor: '#d7edff'}
                                }
                            }
                        },
                        menu: {
                            container: {
                                boxShadow: '#e2e2e2 0px 1px 3px 2px'
                            },
                            item: {
                                hover: {
                                    backgroundColor: '#e1f2ff'
                                },
                                click: {
                                    backgroundColor: '#cfeaff'
                                }
                            },
                            iconContainer: {
                                width: '1.8em'
                            },
                            text: {
                                fontSize: '1.05em'
                            }
                        }
                    }"
                    :connect="{
                        url: `${this.baseUrl}${this.airaBaseUrl}/${this.navBarModel.chatItem.url}/message`,
                        method: 'POST'
                    }"
                    :chatStyle="{ height: '100%', width: '100%' }"
                    :history="[]"
                    :textInput="{
                        placeholder: { text: 'Message AIRA' },
                        styles: {
                           container: {
                            borderRadius: '8px',
                            lineHeight: '1.8em'
                           }
                        }
                    }"
                    :submitButtonStyles="{
                        submit: {
                            container: {
                                default: {
                                    height: '1.7em',
                                    bottom: '1.1em',
                                    right: '0.9em'
                                }
                            },
                            svg: {
                                styles: {
                                    default: {
                                        fontSize: '2rem',
                                        bottom: '1rem'
                                    }
                                }
                            }
                        }
                    }"
                    id="chatElement"
                    ref="chatElementRef"
                    :requestBodyLimits="{ maxMessages: 1 }"
                    style="border-radius: 8px;"
                    :introMessage="{ text: '' }"
                    :messageStyles="{
                        default: {
                            shared: { bubble: { fontSize: '0.75rem', lineHeight: '1.375rem', padding: '0.5rem 0.75rem' } },
                            ai: { bubble: { background: '#F7F1FF' } },
                            user: { bubble: { color: '#fff' } }
                        },
                        image: {
                            user: { bubble: { borderRadius: '1rem', overflow: 'clip', textAlign: 'left', display: 'inline-block' } }
                        },
                        html: {
                           shared: {
                                bubble: {
                                    backgroundColor: 'unset', 
                                    padding: '0px'
                                }
                           }
                        }
                    }">
                </deep-chat>
            </div>
        </div>
    </div>
</template>

<script>
import 'deep-chat';
import NavBarComponent from "./Navigation.vue";

export default {
    components: {
        NavBarComponent
    },
    props: {
        airaBaseUrl: null,
        aiIconUrl: null,
        baseUrl: null,
        usePromptUrl: null,
        navBarModel: null,
        rawHistory: null
    },
    data() {
        return {
            themeColor: "#8107c1",
            themeColorInRgb: "rgb(129, 7, 193)",
            submitButton: null,
            started: false,
            messagesMetadata: new Map(),
            history: []
        }
    },
    mounted() {
        document.onreadystatechange = () => {
            if (document.readyState === "complete") {
                this.main();
            }
        };
    },
    methods: {
        main() {
            setTimeout(() => {
                var modal = document.querySelector('#loading');
                if (modal) {
                    modal.classList.add('is-hidden');
                }
                setTimeout(function () {
                    modal.parentNode.removeChild(modal);
                }, 500);
            }, 1000);

            this.$refs.chatElementRef.onComponentRender = async () => {
                this.setBorders();

                if (!this.started) {
                    this.started = true;
                    document.addEventListener('visibilitychange', function () {
                        if (document.visibilityState === 'visible') {
                            this.$refs.chatElementRef.scrollToBottom();
                        }
                    });

                    this.setRequestInterceptor();
                    this.setOnMessage();
                    this.setResponseInterceptor();
                    this.setHistory();
                }

                const newSubmitButton = this.$refs.chatElementRef.shadowRoot.querySelector('.input-button');
                if (this.submitButton !== newSubmitButton) {
                    this.submitButton = newSubmitButton;
                    this.addClassesToShadowRoot();
                }

                this.bindPromptButtons();
            };
        },
        typeIntoInput(inputElement, text) {
            inputElement.focus();
            inputElement.innerHTML  = "";

            for (let char of text) {
                inputElement.innerHTML  += char;
                inputElement.dispatchEvent(new Event("input", { bubbles: true }));
            }
        },
        bindPromptButtons() {
            this.$refs.chatElementRef.shadowRoot.querySelectorAll('button[prompt-quick-suggestion-button]').forEach(button => {
                button.addEventListener('click', async () => {
                    const text = button.value.valueOf();

                    const buttonGroupId = button.parentNode.getAttribute("prompt-quick-suggestion-button-group-id");

                    this.history = this.history.filter(x => (x.promptQuickSuggestionGroupId === undefined) || x.promptQuickSuggestionGroupId.toString() !== buttonGroupId);
                    this.$refs.chatElementRef.clearMessages(true);

                    this.history.forEach(x => {
                        this.$refs.chatElementRef.addMessage(x);
                    });

                    this.bindPromptButtons();

                    setTimeout(() => {
                        const textInput = this.$refs.chatElementRef.shadowRoot.getElementById("text-input");
                        textInput.classList.remove("text-input-placeholder");

                        this.typeIntoInput(textInput, text);
                    }, 50);

                    const sendUsePromptUrl = `${this.baseUrl}${this.airaBaseUrl}/${this.usePromptUrl}`;
                    await this.removeUsedPromptGroup(buttonGroupId, sendUsePromptUrl);
                });
            });
        },
        async removeUsedPromptGroup(groupId, sendUsePromptUrl) {
            try {
                await fetch(sendUsePromptUrl, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        groupId: groupId,
                    }),
                });
            }
            catch (error) {
                console.error('An error occurred:', error.message);
            }
        },
        setRequestInterceptor() {
            this.$refs.chatElementRef.requestInterceptor = async (requestDetails) => {
                const formData = new FormData();

                this.history.push(requestDetails.body.messages[0]);

                let jsonData = "";

                if (Object.hasOwn(requestDetails.body, 'messages'))
                { 
                    jsonData = requestDetails.body.messages[0].text;
                }
                else
                {
                    const entries = requestDetails.body.entries();
                    if (entries !== null)
                    {
                        let hasMessages = false;
                        for (const [key, value] of requestDetails.body.entries()) {
                            if (key === 'message1')
                            {
                                let parsedValue;
                                try {
                                    parsedValue = JSON.parse(value);
                                } catch (e) {
                                    parsedValue = value;
                                }
                                if (parsedValue && parsedValue.text) {
                                    formData.append('message', parsedValue.text);
                                    hasMessages = true;
                                }
                            }
                            else if (key === 'files') {
                                formData.append(key, value);
                            }
                        }

                        if (!hasMessages) {
                            formData.append('messages', "");
                        }
                    }
                }

                const modifiedRequestDetails = {
                    ...requestDetails,
                    body: jsonData ?? formData,
                    headers: {
                        ...requestDetails.headers
                    },
                };

                return modifiedRequestDetails;
            };
        },
        setResponseInterceptor() {
            this.$refs.chatElementRef.responseInterceptor = (response) => {
                const messageViewModel = this.getMessageViewModel(response);
                
                this.history.push(messageViewModel);

                if (response.quickPrompts.length > 0)
                {
                    this.$refs.chatElementRef.addMessage(messageViewModel);
                    const promptMessage = this.getPromptsViewModel(response);
                    
                    this.history.push(promptMessage);

                    return promptMessage;
                }

                return messageViewModel;
            };
        },
        setOnMessage() {
            this.$refs.chatElementRef.onMessage = (message) => {
                this.bindPromptButtons();
            };
        },
        setBorders(){
            this.$refs.chatElementRef.style.borderLeftStyle = 'none';
            this.$refs.chatElementRef.style.borderTopStyle = 'none';
            this.$refs.chatElementRef.style.borderRightStyle = 'none';
            this.$refs.chatElementRef.style.borderBottomStyle = 'none';
        },
        addClassesToShadowRoot() {
            let shadowRoot = this.$refs.chatElementRef.shadowRoot;

            const style = document.createElement('style');

            style.textContent =
                `.btn-outline-primary {
                    color: ${this.themeColorInRgb};
                    background-color: transparent;
                    border: 1px solid ${this.themeColorInRgb};
                    font-size: 0.75rem;
                    border-radius: 24px;
                    padding: 6px 12px;
                    margin: 4px;
                    transition: color 0.3s ease, background-color 0.3s ease, border-color 0.3s ease;
                    display: inline-block;
                }

                .user-message-text {
                    background-color: ${this.themeColor} !important;
                }

                .btn-outline-primary:hover {
                    color: #fff;
                    background-color: ${this.themeColorInRgb};
                    border-color: ${this.themeColorInRgb};
                }

                .btn-outline-primary:active {
                    color: #fff;
                    background-color: #C64300;
                    border-color: #C64300;
                }

                .btn-outline-primary:disabled {
                        color: ${this.themeColorInRgb};
                    background-color: transparent;
                        border-color: ${this.themeColorInRgb};
                    opacity: 0.65;
                    pointer-events: none;
                }

                .container {
                    display: flex;
                    flex-direction: row;
                    align-items: flex-start;
                    flex-wrap: wrap;
                }

                .message-bubble div {
                    margin-bottom: 8px;
                }

                .message-bubble .btn-outline-primary {
                    margin-right: 8px;
                }
                .message-bubble .deep-chat-button {
                    margin-right: 8px;
                }

                .lds-ring, .lds-ring div {
                    box-sizing: border-box;
                }

                .lds-ring {
                    display: inline-block;
                    position: relative;
                    width: 80px;
                    height: 80px;
                    margin-bottom: 0px;
                }

                .lds-ring div {
                    box-sizing: border-box;
                    display: block;
                    position: absolute;
                    width: 64px;
                    height: 64px;
                    margin: 8px;
                    border: 5px solid currentColor;
                    border-radius: 50%;
                    animation: lds-ring 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite;
                    border-color: currentColor transparent transparent transparent;
                }

                .lds-ring div:nth-child(1) {
                    animation-delay: -0.45s;
                }

                .lds-ring div:nth-child(2) {
                    animation-delay: -0.3s;
                }

                .lds-ring div:nth-child(3) {
                    animation-delay: -0.15s;
                }

                @@keyframes lds-ring {
                    0% {
                        transform: rotate(0deg);
                    }

                    100% {
                        transform: rotate(360deg);
                    }
                }`
            shadowRoot.appendChild(style);
        },
        setHistory() {
            for (const x of this.rawHistory) {
                const messageViewModel = this.getMessageViewModel(x);
                
                this.history.push(messageViewModel);
                this.$refs.chatElementRef.history.push(messageViewModel);
                this.$refs.chatElementRef.addMessage(messageViewModel);

                if (x.quickPrompts.length > 0)
                {
                    const promptMessage = this.getPromptsViewModel(x);
                    this.history.push(promptMessage);
                    this.$refs.chatElementRef.history.push(promptMessage);
                    this.$refs.chatElementRef.addMessage(promptMessage);
                }
            }
        },
        getPromptsViewModel(message) {
            let prompts = `<div prompt-quick-suggestion-button-group-id="${message.quickPromptsGroupId}">`;

            for (var prompt of message.quickPrompts) {
                prompts += `<button class="btn-outline-primary" prompt-quick-suggestion-button value="${prompt}">${prompt}</button>`;
            }

            prompts += '</div>';

            return {
                role: 'ai',
                html: prompts,
                promptQuickSuggestionGroupId: `${message.quickPromptsGroupId}`
            }
        },
        getMessageViewModel(message) {
            if (message.url !== null) {
                return {
                    role: "user",
                    files: [
                        {
                            src: message.url,
                            type: "image"
                        }
                    ]
                };
            }

            return {
                role: message.role,
                text: message.message
            }
        },
        isJSONWithProperty(string, property) {
            try {
                const json = JSON.parse(string);
                return json && typeof json === 'object' && property in json;
            } catch (e) {
                return false;
            }
        }
    }
};
</script>