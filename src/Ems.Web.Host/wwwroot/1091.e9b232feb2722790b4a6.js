(window.webpackJsonp=window.webpackJsonp||[]).push([[1091],{obg2:function(e,r,n){"use strict";n.r(r),n.d(r,"skLocale",(function(){return o}));var s=n("HPA8");function t(e){return e>1&&e<5&&1!=~~(e/10)}function a(e,r,n,s){var a=e+" ";switch(n){case"s":return r||s?"pár sekúnd":"pár sekundami";case"ss":return r||s?a+(t(e)?"sekundy":"sekúnd"):a+"sekundami";case"m":return r?"minúta":s?"minútu":"minútou";case"mm":return r||s?a+(t(e)?"minúty":"minút"):a+"minútami";case"h":return r?"hodina":s?"hodinu":"hodinou";case"hh":return r||s?a+(t(e)?"hodiny":"hodín"):a+"hodinami";case"d":return r||s?"deň":"dňom";case"dd":return r||s?a+(t(e)?"dni":"dní"):a+"dňami";case"M":return r||s?"mesiac":"mesiacom";case"MM":return r||s?a+(t(e)?"mesiace":"mesiacov"):a+"mesiacmi";case"y":return r||s?"rok":"rokom";case"yy":return r||s?a+(t(e)?"roky":"rokov"):a+"rokmi"}}var o={abbr:"sk",months:"január_február_marec_apríl_máj_jún_júl_august_september_október_november_december".split("_"),monthsShort:"jan_feb_mar_apr_máj_jún_júl_aug_sep_okt_nov_dec".split("_"),weekdays:"nedeľa_pondelok_utorok_streda_štvrtok_piatok_sobota".split("_"),weekdaysShort:"ne_po_ut_st_št_pi_so".split("_"),weekdaysMin:"ne_po_ut_st_št_pi_so".split("_"),longDateFormat:{LT:"H:mm",LTS:"H:mm:ss",L:"DD.MM.YYYY",LL:"D. MMMM YYYY",LLL:"D. MMMM YYYY H:mm",LLLL:"dddd D. MMMM YYYY H:mm",l:"D. M. YYYY"},calendar:{sameDay:"[dnes o] LT",nextDay:"[zajtra o] LT",nextWeek:function(e){switch(Object(s.a)(e)){case 0:return"[v nedeľu o] LT";case 1:case 2:return"[v] dddd [o] LT";case 3:return"[v stredu o] LT";case 4:return"[vo štvrtok o] LT";case 5:return"[v piatok o] LT";case 6:return"[v sobotu o] LT"}},lastDay:"[včera o] LT",lastWeek:function(e){switch(Object(s.a)(e)){case 0:return"[minulú nedeľu o] LT";case 1:case 2:return"[minulý] dddd [o] LT";case 3:return"[minulú stredu o] LT";case 4:case 5:return"[minulý] dddd [o] LT";case 6:return"[minulú sobotu o] LT"}},sameElse:"L"},relativeTime:{future:"o %s",past:"pred %s",s:a,ss:a,m:a,mm:a,h:a,hh:a,d:a,dd:a,M:a,MM:a,y:a,yy:a},dayOfMonthOrdinalParse:/\d{1,2}\./,ordinal:"%d.",week:{dow:1,doy:4}}}}]);