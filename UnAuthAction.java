package com.gss.adm.web.jsf.actions;


import javax.ws.rs.Consumes;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.core.MediaType;

import org.apache.commons.lang3.Validate;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;

import com.gss.adm.api.exception.DecryptErrorException;
import com.gss.adm.api.exception.MailSenderAuthenticationException;
import com.gss.adm.core.model.User;
import com.gss.adm.core.service.UserService;
import com.gss.adm.web.view.UserChangePasswordView;
import com.gss.adm.web.view.UserRegisterView;

/**
 * @author Hank_Lin
 */

@Component
@Path("unAuth")
public class UnAuthAction {

    @Autowired
    private UserService userService;

    @POST
    @Path("user/save")
    @Consumes(MediaType.APPLICATION_JSON)
    public Boolean save(UserRegisterView userRegisterView){
    	
    	Validate.notEmpty(userRegisterView.getUserName());
    	Validate.notEmpty(userRegisterView.getPassword());
    	Validate.notEmpty(userRegisterView.getLocalName());
    	Validate.notEmpty(userRegisterView.getName());
    	Validate.notEmpty(userRegisterView.getEmail());
        
        boolean isExists = userService.exists(userRegisterView.getUserName());
        boolean checkEmailIsExists = userService.checkEmailIsExists(userRegisterView.getEmail());
        if( ! isExists && ! checkEmailIsExists){
            User user = convert(userRegisterView);
            userService.registerUser(user);
        } else {
            return false;
        }
        
        return true;
    }
    
	@POST
	@Path("user/forgetPassword")
	@Consumes(MediaType.APPLICATION_JSON)
	public Boolean sendEmail(UserChangePasswordView userChangePasswordView)
			throws DecryptErrorException, MailSenderAuthenticationException {

		Validate.notEmpty(userChangePasswordView.getUserName());
		Validate.notEmpty(userChangePasswordView.getEmail());

		String userName = userChangePasswordView.getUserName();
		String email = userChangePasswordView.getEmail();

		return userService.sendMailByUserNameAndEmail(userName, email);
	}

    /////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////private method//////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////
   
    private User convert(UserRegisterView source){
        
        User target = new User();
        target.setUsername(source.getUserName());
        target.setPassword(source.getPassword());
        target.setLocalName(source.getLocalName());
        target.setName(source.getName());
        target.setEmail(source.getEmail());
        
        return target;
    }
}
