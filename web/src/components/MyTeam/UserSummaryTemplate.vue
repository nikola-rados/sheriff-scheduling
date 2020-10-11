<template> 
    <b-card align="center">
                <b-card-img
                    v-if="photo"
                    :src="photo"
                    style="max-width: 20rem; max-height: 22rem;"
                    class="mb-2"
                ></b-card-img>
            
                <b-icon-person-circle v-else class="mb-3" variant="secondary" font-scale="7.5"></b-icon-person-circle>
                
                <b-card no-body class="mb-3 mt-2" v-if="editMode">
                     <h3><b-badge v-if="photoError" variant="danger"> {{photoErrorMsg}} <b-icon class="ml-1" icon = x-square-fill @click="photoError = false" /></b-badge></h3>
                                       
                    <label class="btn btn-default btnfile">
                        Browse for File <input type="file" style="display: none;" accept="image/x-png,image/gif,image/jpeg" @change="onFileSelected">
                    </label>
                </b-card>

                <b-card-sub-title>{{userBadgeNumber}}</b-card-sub-title>
                <b-card-title>{{userName}}</b-card-title>
                <b-card-sub-title>{{userRole|capitilize}}</b-card-sub-title>
                
                
           
    </b-card>   
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';
    
    import "@store/modules/CommonInformation";  
    import {locationInfoType} from '../../types/common';  
    const commonState = namespace("CommonInformation");

    @Component
    export default class UserSummaryTemplate extends Vue {

        @commonState.State
        public location!: locationInfoType;

        @Prop({required: true})
        userBadgeNumber!: string;

        @Prop({required: true})
        userName!: string;

        @Prop({required: true})
        userRole!: string;

        @Prop({required: true})
        editMode!: boolean;

        @Prop({required: true})
        userImage!: string;

        @commonState.State
        public token!: string;
        

        photo = ''
        photoError = false
        photoErrorMsg = ''

        mounted()
        {
            this.photo = this.userImage;
        }

        
        public onFileSelected(event)
        {
            this.photoError = false
            this.photoErrorMsg = ''
            console.log(event)
            if (event.target.files && event.target.files[0]) 
            {       
                let imageData: string | ArrayBuffer | null;         
                const reader = new FileReader();                
                reader.onload = (e) => {
                        
                    if(e && e.target)
                        imageData = e.target.result;
                }
                reader.readAsArrayBuffer(event.target.files[0]);
                console.log(event.target.files[0])

                console.log(event.target.files[0])

                const acceptableImageTypes = ["image/png","image/gif","image/jpeg"]

                reader.onloadend = (()=>{

                    if(acceptableImageTypes.includes(event.target.files[0].type))
                    {                    
                        const imageBlob = new Blob([imageData?imageData:'']);   
                        const formData = new FormData();
                        formData.append('file', imageBlob);
            
                        const url = 'api/sheriff/uploadphoto?id=d27ffe54-595f-4cfb-b42e-597129b80e18&badgeNumber=007'
                        const options = {headers:{'Authorization' :'Bearer '+this.token, 'Content-Type': 'multipart/form-data'}}           
                        
                        this.$http.post(url, formData, options )
                            .then(response => {
                                console.log(response)
                                this.photo = 'data:image/;base64,'+response.data.photo
                            
                            }, err => {
                                console.log(err);
                                this.photoError = true;
                                this.photoErrorMsg = 'Photo size is large!';
                            }) 
                    }
                    else
                    {
                        console.log('EErr')
                        this.photoError = true
                        this.photoErrorMsg = 'Only .jpg , .png , .gif photos!'
                    }
                })
            
            }
        }


    }
</script>

 <style scoped>   

    .card {
        border: white;
    }

    .btnfile {
        position: relative;
        overflow: hidden;
        border: 1px solid;
        background: lightgrey;
    }
    .btnfile:hover {
        background-color: #103c6b;
        color: white;
    }
    
   


</style>