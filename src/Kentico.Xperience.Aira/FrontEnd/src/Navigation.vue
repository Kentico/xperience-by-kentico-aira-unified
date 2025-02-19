<template>
    <nav class="navbar">
        <div class="container">
            <div class="navbar-brand">
                <img alt="Kentico" id="aira-app-logo" :src="`${this.baseUrl}${this.navBarModel.logoImgRelativePath}`">  
            </div>
            <h1 class="c-app_title" id="aira-title">
                <svg id="aira-title-img" class="c-icon" width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M5.61629 10.1095L7.48839 4.49316L9.36049 10.1095L14.9768 11.9816L9.36049 13.8537L7.48839 19.4699L5.61629 13.8537L0 11.9816L5.61629 10.1095Z" fill="currentColor"/>
                    <path d="M13.8542 18.3463L14.9774 14.9766L16.1007 18.3463L19.4704 19.4696L16.1007 20.5929L14.9774 23.9626L13.8542 20.5929L10.4844 19.4696L13.8542 18.3463Z" fill="currentColor"/>
                    <path d="M16.4735 4.49303L17.9712 0L19.4689 4.49303L23.9619 5.99071L19.4689 7.48839L17.9712 11.9814L16.4735 7.48839L11.9805 5.99071L16.4735 4.49303Z" fill="currentColor"/>
                </svg>

<!--                <img alt="Kentico" id="aira-title-img" class="c-icon text-primary" :src="`${this.baseUrl}${this.navBarModel.titleImagePath}`">-->
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
                            <a class="d-flex align-items-center gap-2 btn" :href="`${this.baseUrl}${this.airaBaseUrl}/${this.navBarModel.chatItem.url}`">
                                <img :src="`${this.baseUrl}${this.navBarModel.chatItem.menuImage}`" class="c-icon text-primary" />
                                {{`${this.navBarModel.chatItem.title}`}}
                            </a>
                            <a class="d-flex align-tems-center gap-2 btn" :href="`${this.baseUrl}${this.airaBaseUrl}/${this.navBarModel.smartUploadItem.url}`">
                                <img :src="`${this.baseUrl}${this.navBarModel.smartUploadItem.menuImage}`" class="c-icon text-primary" />
                                {{`${this.navBarModel.smartUploadItem.title}`}}
                            </a>
                        </div>

                        <hr class="c-nav_hr">
                        <div class="c-nav_footer">
                            <p class="text-center fs-1">
                                {{`${this.navBarModel.menuMessage}`}}
                            </p>
                            <p id="appVersion" class="mt-3"></p>
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
    components: {
    },
    props: {
        baseUrl: null,
        navBarModel: null,
        airaBaseUrl: null
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
        this.main();
    },
    methods: {
        main() {
            
        },
    }
}
</script>