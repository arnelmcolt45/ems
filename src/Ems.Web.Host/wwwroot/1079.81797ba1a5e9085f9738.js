(window.webpackJsonp=window.webpackJsonp||[]).push([[1079],{"V+Dp":function(e,r,n){"use strict";n.r(r),n.d(r,"huLocale",(function(){return _}));var s=n("HPA8"),a="vasárnap hétfőn kedden szerdán csütörtökön pénteken szombaton".split(" ");function t(e,r,n,s){switch(n){case"s":return s||r?"néhány másodperc":"néhány másodperce";case"ss":return e+(s||r?" másodperc":" másodperce");case"m":return"egy"+(s||r?" perc":" perce");case"mm":return e+(s||r?" perc":" perce");case"h":return"egy"+(s||r?" óra":" órája");case"hh":return e+(s||r?" óra":" órája");case"d":return"egy"+(s||r?" nap":" napja");case"dd":return e+(s||r?" nap":" napja");case"M":return"egy"+(s||r?" hónap":" hónapja");case"MM":return e+(s||r?" hónap":" hónapja");case"y":return"egy"+(s||r?" év":" éve");case"yy":return e+(s||r?" év":" éve")}return""}function u(e,r){return(r?"":"[múlt] ")+"["+a[Object(s.a)(e)]+"] LT[-kor]"}var _={abbr:"hu",months:"január_február_március_április_május_június_július_augusztus_szeptember_október_november_december".split("_"),monthsShort:"jan_feb_márc_ápr_máj_jún_júl_aug_szept_okt_nov_dec".split("_"),weekdays:"vasárnap_hétfő_kedd_szerda_csütörtök_péntek_szombat".split("_"),weekdaysShort:"vas_hét_kedd_sze_csüt_pén_szo".split("_"),weekdaysMin:"v_h_k_sze_cs_p_szo".split("_"),longDateFormat:{LT:"H:mm",LTS:"H:mm:ss",L:"YYYY.MM.DD.",LL:"YYYY. MMMM D.",LLL:"YYYY. MMMM D. H:mm",LLLL:"YYYY. MMMM D., dddd H:mm"},meridiemParse:/de|du/i,isPM:function(e){return"u"===e.charAt(1).toLowerCase()},meridiem:function(e,r,n){return e<12?!0===n?"de":"DE":!0===n?"du":"DU"},calendar:{sameDay:"[ma] LT[-kor]",nextDay:"[holnap] LT[-kor]",nextWeek:function(e){return u(e,!0)},lastDay:"[tegnap] LT[-kor]",lastWeek:function(e){return u(e,!1)},sameElse:"L"},relativeTime:{future:"%s múlva",past:"%s",s:t,ss:t,m:t,mm:t,h:t,hh:t,d:t,dd:t,M:t,MM:t,y:t,yy:t},dayOfMonthOrdinalParse:/\d{1,2}\./,ordinal:"%d.",week:{dow:1,doy:4}}}}]);