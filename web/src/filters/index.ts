import Vue from 'vue'
import moment from 'moment-timezone';


Vue.filter('beautify-date', function(date){
	enum MonthList {'Jan' = 1, 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'}
	if(date)
		return date.substr(8,2) + ' ' + MonthList[Number(date.substr(5,2))] + ' ' + date.substr(0,4);
	else
		return ''
})

Vue.filter('beautify-date-time', function(date){
	enum MonthList {'Jan' = 1, 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'}
	if(date)
		return date.substr(8,2) + ' ' + MonthList[Number(date.substr(5,2))] + ' ' + date.substr(0,4)+ ' ' + date.substr(11,5);
	else
		return ''
})

Vue.filter('beautify-time', function(date){
	
	if(date)
		return date.substr(11,5);
	else
		return ''
})

Vue.filter('capitilize', function(str: string){
	
	if(str)
		return str.charAt(0).toUpperCase() + (str.slice(1)).toLowerCase();
	else
		return ''
	
})

Vue.filter('truncate', function (text: string, stop: number) {
    return text.slice(0, stop) + (stop < text.length ? '...' : '')
})

Vue.filter('convertTime', function(time: string, type: string){
	const tail="00:00:00.000Z";
	if(!time && type=='Start') return tail;
	else if(!time && type=='End') return "23:59:00.000Z";
	else if(time.length==1) return '0'+time+ tail.slice(2);
	else if(time.length==4) return time.slice(0,3)+'0'+time.slice(3,4)+ tail.slice(5);
	else  return time+ tail.slice(time.length);
})

Vue.filter('convertDate', function(date: string, time: string, type: string){
	const tail="00:00:00.000Z";
	let result = "";
	if(!time && type=='StartTime') result = date+'T'+tail;
	else if(!time && type=='EndTime') result = date+"T23:59:00.000Z";
	else if(time.length==1) result = date+'T0'+time+ tail.slice(2);
	else if(time.length==4) result = date+'T'+time.slice(0,3)+'0'+time.slice(3,4)+ tail.slice(5);
	else  result = date+'T'+time+ tail.slice(time.length);
	return result //moment(result).tz("America/Vancouver").format();
})