<template>
    <div>
        <h3 class="mx-4 font-weight-normal"> {{pageHeaderText}} </h3>
        <hr class="mx-3 bg-light" style="height: 5px;"/> 
    </div>
</template>


<script lang="ts">
  import { Component, Prop, Vue } from 'vue-property-decorator';

  @Component
  export default class PageHeader extends Vue {

      @Prop({required: true})
      pageHeaderText!: string;

  }

  </script>