import { DirectiveOptions } from  'vue';
import Sortable from 'sortablejs';
import store from "../../../store"; 

const sortLeaveTrainingType: DirectiveOptions = {
    bind(el,  binding, vnode) {
        const tableRows = el.querySelector('tbody');
         Sortable.create(tableRows, {        
            animation: 50,
            handle: '.handle', 
            chosenClass: 'is-selected',
            sort: true,

            onEnd: function(evt: any) {
                store.commit('ManageTypesInformation/setSortingLeaveTrainingInfo',{prvIndex: evt.oldIndex, newIndex: evt.newIndex} );
            }
        })
    }
}

export default sortLeaveTrainingType;