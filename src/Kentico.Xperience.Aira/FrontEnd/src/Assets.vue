<template>
    <div class="c-app_inner" v-bind:class="{'is-loaded':isLoaded}">
        <div id="loading" class="c-loading">
            <img :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/spinner.svg`" alt="loading spinner"
                 class="c-loading_spinner"/>
        </div>

        <div class="c-app_header">
            <NavBarComponent :airaBaseUrl="airaBaseUrl" :baseUrl="baseUrl" :navBarModel="navBarModel"/>
        </div>

        <!--        <template v-if="phase == 'uploading'">
                    <div id="loading" class="c-loading">
                        <img :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/spinner.svg`" class="c-loading_spinner" alt="loading spinner" />
                    </div>

                    <div class="c-done-page-layout">
                        <div>
                            <h2>Thank you<span class="text-primary">.</span></h2>
                        </div>
                        <div>
                            <svg viewBox="0 0 78.400003 58.227815"
                                 xmlns="http://www.w3.org/2000/svg" class="c-checkmark">
                                <polyline class="path"
                                        fill="none"
                                        stroke-width="8"
                                        stroke-linecap="round"
                                        stroke-miterlimit="10"
                                        stroke-dasharray="100"
                                        stroke-dashoffset="-100"
                                        points="74.4,4 25.7,52.6 4,31.5"
                                        id="polyline1" />
                            </svg>
                        </div>
                        <div>
                            <p>
                                Your images have been successfully uploaded.
                            </p>
                        </div>
                        <div>
                            <a href="/aira/assets" class="btn btn-primary text-uppercase">
                                <img :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons/plus-circle.svg`" class="c-icon fs-6" />
                                upload more
                            </a>
                        </div>
                    </div>
                </template>-->

        <div class="c-app_body">
            <div class="container">
                <form>
                    <input ref="fileInput" accept=".jpg,.jpeg,.png,.bmp" class="d-none" hidden multiple type="file">
                </form>
                <template v-if="phase == 'empty'">
                    <div class="c-empty-page-layout">
                        <div>
                            <p>It looks empty, please upload your images here.</p>
                        </div>
                        <div>
                            <img :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/image-placeholder.svg`"
                                 alt="image placeholder" class="img-fluid">
                        </div>
                        <div>
                            <button class="btn btn-secondary bigger-icon" @click="pickImage();">
                                <svg class="c-icon" width="16" height="20" viewBox="0 0 16 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path fill="currentColor" d="M2.98413 13.1232C2.98413 12.778 3.26395 12.4982 3.60913 12.4982H5.49817V10.6604C5.49817 10.3152 5.77799 10.0354 6.12317 10.0354C6.46835 10.0354 6.74817 10.3152 6.74817 10.6604V12.4982H8.60913C8.95431 12.4982 9.23413 12.778 9.23413 13.1232C9.23413 13.4683 8.95431 13.7482 8.60913 13.7482H6.74817V15.6604C6.74817 16.0056 6.46835 16.2854 6.12317 16.2854C5.77799 16.2854 5.49817 16.0056 5.49817 15.6604V13.7482H3.60913C3.26395 13.7482 2.98413 13.4683 2.98413 13.1232Z"/>
                                    <path fill="currentColor" fill-rule="evenodd" clip-rule="evenodd" d="M1.75 0H9.49848L15.4975 5.71426V18.7499C15.4975 19.4403 14.9378 19.9999 14.2475 19.9999H1.75C1.05964 19.9999 0.5 19.4403 0.5 18.7499V1.25C0.5 0.559643 1.05964 0 1.75 0ZM14.2475 18.7499H1.75L1.75 1.25L8 1.25V5.61951C8 6.65504 8.83947 7.49451 9.875 7.49451H14.2475V18.7499ZM14.2418 6.24451L9.25 1.48964V5.61951C9.25 5.96468 9.52982 6.24451 9.875 6.24451H14.2418Z"/>
                                </svg>
                                UPLOAD ASSETS
                            </button>
                        </div>
                    </div>
                </template>

                <template v-if="phase == 'selection' || phase == 'uploading'">
                    <div class="row g-2.5">
                        <div v-for="(file, index) in files" :key="index" class="col-6">
                            <div class="c-uploaded-image">
                                <div class="c-uploaded-image_header">
                                    <div v-if="phase == 'selection'" class="c-uploaded-image_check">
                                        <img
                                            :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons/check-white.svg`"
                                            alt="check" class="c-icon"/>
                                    </div>
                                    <button aria-label="Remove" class="btn btn-remove-img"
                                            @click="removeFile2(file, index);">
                                        <img
                                            :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons/cross.svg`"
                                            alt="" class="c-icon"/>
                                    </button>
                                </div>
                                <img alt="uploaded image" class="c-uploaded-image_img"
                                     v-bind:src="createObjectURL(file)">
                            </div>
                        </div>
                    </div>
                </template>
                <template v-if="phase == 'selection'">
                    <div class="c-empty-page-layout px-0">
                        <div class="d-flex flex-wrap justify-content-center gap-2">
                            <button class="btn btn-outline-secondary bigger-icon" @click="pickImage();">
                                <svg class="c-icon" width="16" height="20" viewBox="0 0 16 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path fill="currentColor" d="M2.98413 13.1232C2.98413 12.778 3.26395 12.4982 3.60913 12.4982H5.49817V10.6604C5.49817 10.3152 5.77799 10.0354 6.12317 10.0354C6.46835 10.0354 6.74817 10.3152 6.74817 10.6604V12.4982H8.60913C8.95431 12.4982 9.23413 12.778 9.23413 13.1232C9.23413 13.4683 8.95431 13.7482 8.60913 13.7482H6.74817V15.6604C6.74817 16.0056 6.46835 16.2854 6.12317 16.2854C5.77799 16.2854 5.49817 16.0056 5.49817 15.6604V13.7482H3.60913C3.26395 13.7482 2.98413 13.4683 2.98413 13.1232Z"/>
                                    <path fill="currentColor" fill-rule="evenodd" clip-rule="evenodd" d="M1.75 0H9.49848L15.4975 5.71426V18.7499C15.4975 19.4403 14.9378 19.9999 14.2475 19.9999H1.75C1.05964 19.9999 0.5 19.4403 0.5 18.7499V1.25C0.5 0.559643 1.05964 0 1.75 0ZM14.2475 18.7499H1.75L1.75 1.25L8 1.25V5.61951C8 6.65504 8.83947 7.49451 9.875 7.49451H14.2475V18.7499ZM14.2418 6.24451L9.25 1.48964V5.61951C9.25 5.96468 9.52982 6.24451 9.875 6.24451H14.2418Z"/>
                                </svg>
                                UPLOAD ASSETS
                            </button>
                            <button :disabled="!formIsValid" class="btn btn-secondary" type="button"
                                    @click="fireUpload()">
                                SAVE
                            </button>
                        </div>
                    </div>
                </template>
                <template v-if="phase == 'uploading'">
                    <div v-if="!alertHidden" class="c-alert mt-3" role="alert">
                        <div class="c-alert_header">
                            <img
                                :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons/check-circle-green.svg`"
                                alt="" class="c-icon"/>
                        </div>
                        <div class="c-alert_body">
                            <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Nihil optio lorem.</p>
                        </div>
                        <div class="c-alert_footer">
                            <button aria-label="Remove" class="btn btn-remove-img"
                                    @click="alertHidden = true;">
                                <img
                                    :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons/cross.svg`"
                                    alt="" class="c-icon"/>
                            </button>
                        </div>
                    </div>
                    <div class="c-empty-page-layout px-0">
                        <div class="d-flex flex-wrap justify-content-center gap-2 mt-3">
                            <button class="btn btn-secondary" @click="pickImage();">
                                <img :src="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons/plus-circle.svg`"
                                     class="c-icon fs-6"/>
                                UPLOAD ASSETS
                            </button>
                        </div>
                    </div>
                </template>
            </div>
        </div>
    </div>
</template>

<script>
import NavBarComponent from "./Navigation.vue";

export default {
    components: {
        NavBarComponent
    },
    props: {
        airaBaseUrl: null,
        baseUrl: null,
        navBarModel: null
    },
    data() {
        return {
            files: [],
            phase: 'empty', // 'uploading', 'selection', 'empty'
            alertHidden: false,
            formIsValid: true,
            uploading: false,
            filesPromiseResolve: null,
            filesPromise: null,

            isLoaded: true
        }
    },
    mounted() {
        document.onreadystatechange = () => {
            if (document.readyState === "complete") {
                this.main();
            }
        }
    },
    methods: {
        main() {
            this.validateState();
            this.$refs.fileInput.onchange = this.fileInputChange;

            setTimeout(() => {
                var modal = document.querySelector('#loading');
                if (modal) {
                    modal.classList.add('is-hidden');
                }
                setTimeout(function () {
                    modal.parentNode.removeChild(modal);
                }, 500);
            }, 1000);
        },
        async pickImage(event) {
            this.phase = 'selection';
            this.filesPromise = new Promise(resolve => this.filesPromiseResolve = resolve);
            this.$refs.fileInput.click();
            const files = await this.filesPromise;
            this.filesPromise = null;
            this.filesPromiseResolve = null;
            Array.from(files).forEach(file => {
                this.files.push(file);
            });
            this.validateState();
        },
        createObjectURL(file) {
            return URL.createObjectURL(file);
        },
        fileInputChange(e) {
            this.filesPromiseResolve(e.target.files);
        },
        validateState() {
            this.formIsValid = this.files.length > 0;
        },
        removeFile2(file, index) {
            this.files.splice(index, 1);
            this.validateState();
        },
        fireUpload() {
            const formData = new FormData();

            this.files.forEach((f) => {
                formData.append('files', f);
            });

            fetch(`${this.baseUrl}${this.airaBaseUrl}/${this.navBarModel.smartUploadItem.url}/upload`, {
                method: 'POST',
                body: formData,
                mode: "same-origin",
                credentials: "same-origin",
            })
                .then(async (r) => {
                    this.phase = 'uploading';

                    setTimeout(() => {
                        var modal = document.querySelector('#loading');

                        if (modal) {
                            modal.classList.remove('is-hidden');
                        }
                        setTimeout(function () {
                            modal.parentNode.removeChild(modal);
                            setTimeout(() => {
                                if (document.querySelector(".c-checkmark")) {
                                    document.querySelector(".c-checkmark").classList.add("do-animation");
                                }
                            })
                        }, 500);
                    }, 1000);
                });
        }
    }
}


</script>