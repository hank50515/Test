package com.gss.isbg.gssdlc.acl.portal.web.controller;

import java.util.Locale;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.MessageSource;
import org.springframework.context.i18n.LocaleContextHolder;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.servlet.i18n.CookieLocaleResolver;

import com.google.common.collect.HashBasedTable;
import com.google.common.collect.Table;
import com.gss.isbg.gssdlc.acl.portal.web.view.I18nProperty;
import com.gss.tds.common.web.utils.XSSPreventionUtils;
/**
* Author : Hank
*/
@Controller
@RequestMapping("global/test")
public class GlobalController {

        @Autowired
        private MessageSource messageSource;

        @Autowired
        protected CookieLocaleResolver resolver;

        private Table<String, Locale, I18nProperty> i18nPropertiesCacheMap = HashBasedTable.create();

		/** i18n */
        @ResponseBody
        @RequestMapping(value = "/i18n", method = RequestMethod.GET)
        public I18nProperty getProperty(@RequestParam("key") String key) {

                Locale locale = LocaleContextHolder.getLocale();
                I18nProperty property = i18nPropertiesCacheMap.get(key, locale);
                if ( property != null ) {
                        return property;
                }

                try {
                        String value = messageSource.getMessage(key, null, locale);
                        property = new I18nProperty(key, value);
                        i18nPropertiesCacheMap.put(key, locale, property);
                        return XSSPreventionUtils.encode(property);
                } catch (Exception e) {
                        return new I18nProperty();
                }
        }
		
		// GoodBye
		@RequestMapping("goodBye.htm")
		public ModelAndView goodBye(HttpServletRequest request,
				HttpServletResponse response) throws Exception {
	 
			ModelAndView model = new ModelAndView("helloWorld");
			model.addObject("msg", "goodBye()");
			return model;
		}
		
		/**
		*
		* save
		*/
		@RequestMapping(value = "/save", method = RequestMethod.POST)
		public ModelAndView save(@ModelAttribute("contactForm") ContactForm contactForm) {
			System.out.println(contactForm);
			System.out.println(contactForm.getContacts());
			List<Contact> contacts = contactForm.getContacts();
			
			if(null != contacts && contacts.size() > 0) {
				ContactController.contacts = contacts;
				for (Contact contact : contacts) {
					System.out.printf("%s \n", contact.getName());
				}
			}
			
			return new ModelAndView("show_contact", "contactForm", contactForm);
		}
		
		@RequestMapping(value = "/save1", method = RequestMethod.POST)
		public ProjectView finAllProjects() {
			ProjectView projectView = new ProjectView();
			return projectView
		}
}