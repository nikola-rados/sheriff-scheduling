import Vue from 'vue'
import moment from 'moment-timezone';


Vue.filter('beautify-date', function(date){
	enum MonthList {'Jan' = 1, 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'}
	if(date)
		return date.substr(8,2) + ' ' + MonthList[Number(date.substr(5,2))] + ' ' + date.substr(0,4);
	else
		return ''
})

Vue.filter('beautify-full-date', function(date){
	
	if(date)
		return moment(date).format('MMMM DD, YYYY');
	else
		return ''
})

Vue.filter('beautify-date-weekday', function(date){
	if(date)
		return	moment(date).format('ddd DD MMM YYYY');
	else
		return ''
})

Vue.filter('beautify-date-time-weekday', function(date){
	if(date)
		return	moment(date).format('dddd MMM DD, YYYY HH:mm');
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

Vue.filter('capitalize', function(str: string){
	
	if(str)
		return str.charAt(0).toUpperCase() + (str.slice(1)).toLowerCase();
	else
		return ''
	
})

Vue.filter('capitalizefirst', function(str: string){
	
	if(str)
		return str.charAt(0).toUpperCase() + (str.slice(1));
	else
		return ''
	
})

Vue.filter('truncate', function (text: string, stop: number) {
	if(text)
		return (stop+3 < text.length) ? text.slice(0, stop)+'...' : text
	else
		return ''
})

Vue.filter('truncateleft', function (text: string, stop: number) {
    return (stop+3 < text.length) ? '...'+text.slice(0, stop) : text
})

Vue.filter('convertDate', function(date: string, time: string, type: string, timezone: string){
	const tail="00:00:00.000";
	let result = "";
	if(!time && type=='StartTime') result = date+'T'+tail;
	else if(!time && type=='EndTime') result = date+"T23:59:00.000";
	else if(time.length==1) result = date+'T0'+time+ tail.slice(2);
	else  result = date+'T'+time+ tail.slice(time.length);
	return moment.tz(result, timezone).utc().format();
})

Vue.filter('isDateFullday', function(startDate: string, endDate: string){
	const tail="1900-01-01T00:00:00";
	const start = moment(startDate+ tail.slice(startDate.length)); 
	const end = moment(endDate+ tail.slice(endDate.length));
	const duration = moment.duration(end.diff(start));
	if(duration.asMinutes() < 1439 && duration.asMinutes()> -1439 ){
		return false; 
	}else{
		return true;
	}	 	
})