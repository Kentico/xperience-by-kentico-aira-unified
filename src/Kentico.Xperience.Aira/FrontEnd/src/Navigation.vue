<template>
    <nav class="navbar">
        <div class="container">
            <div class="navbar-brand">
                <img alt="Kentico" id="aira-app-logo" :src="`${this.baseUrl}${this.navBarModel.logoImgRelativePath}`">  
            </div>
            <h1 class="c-app_title" id="aira-title">
                <img alt="Kentico" id="aira-title-img" class="c-icon text-primary" :src="`${this.baseUrl}${this.navBarModel.titleImagePath}`">
                <span id="aira-title-text">
                    {{ `${this.navBarModel.titleText}` }}
                </span>
            </h1>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNavAltMarkup" aria-controls="navbarNavAltMarkup" aria-expanded="false" aria-label="Toggle navigation">
                <svg class="c-icon" id="burger-icon" :style="{ color: `${this.themeColor}` }">
                    <use :xlink:href="`${this.baseUrl}/_content/Kentico.Xperience.Aira/img/icons.svg#hamburger-menu`"></use>
                </svg>
            </button>
        </div>

        <div class="collapse navbar-collapse" id="navbarNavAltMarkup">
            <div class="container">
                <div class="navbar-nav">
                    <div class="c-nav">
                        <div class="c-nav_content" id="aira-menu-submenu-items">
                            <a class="d-flex align-items-center gap-2 btn" :href="`${this.baseUrl}${this.airaPathBase}/${this.navBarModel.chatItem.url}`">
                                <img :src="`${this.baseUrl}${this.navBarModel.chatItem.imagePath}`" class="c-icon text-primary" />
                                {{`${this.navBarModel.chatItem.title}`}}
                            </a>
                            <a class="d-flex align-tems-center gap-2 btn" :href="`${this.baseUrl}${this.airaPathBase}/${this.navBarModel.chatItem.url}`">
                                <img :src="`${this.baseUrl}${this.navBarModel.smartUploadItem.imagePath}`" class="c-icon text-primary" />
                                {{`${this.navBarModel.smartUploadItem.title}`}}
                            </a>
                        </div>

                        <hr class="c-nav_hr">
                        <div class="c-nav_content">
                            <p id="nav-content"></p>
                        </div>
                        <div class="c-nav_footer">
                            <p id="appVersion"></p>
                            <p>© 2024 Kentico. All rights reserved.</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </nav>
</template>

<script>
export default {
    props: {
        baseUrl: null,
        navBarModel: null,
        airaPathBase: null
    },
    data() {
        return {
            themeColor: "#8107c1",
            themeColorInRgb: "rgb(129, 7, 193)",
            submitButton: null,
            started: false
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

        }
    }
}
</script>