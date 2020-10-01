<template>
    <b-card bg-variant="white" class="home">    
      
        <b-card  align="left" >
            
            <b-card >
                <b-card-title class="h2"> Temporary landing page for Sheriff Scheduling </b-card-title>
            </b-card>

            <label for="file-input">
                <img class="preview" style="display: block;width: 400px;height: 400px;" :src="imageData">
            </label>
            <input id="file-input" type="file" style=" display: none;" name="photo" @change="onFileSelected">

            <b-button @click="save"> Save </b-button>

        </b-card>
    </b-card>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { namespace } from 'vuex-class'
    import axios from 'axios'

    @Component
    export default class Home extends Vue {

        imageData: null|string|ArrayBuffer = null;

        
        public onFileSelected(event)
        {
            console.log(event)
            if (event.target.files && event.target.files[0]) 
            {                
                const reader = new FileReader();                
                reader.onload = (e) => {
                        
                    if(e && e.target)
                        this.imageData = e.target.result;
                }
                reader.readAsDataURL(event.target.files[0]);
            }
        }

        public save()
        {
            console.log(this.imageData)         
        }




    }
</script>

<style scoped>
    .card {
        border: white;
    }
</style>
