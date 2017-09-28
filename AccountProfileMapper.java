package tw.gov.tipo.patkm.mapper;

import java.math.BigDecimal;

import org.apache.ibatis.annotations.Param;
import org.springframework.stereotype.Repository;

import tw.gov.tipo.patkm.entity.AccountProfile;
/**
 * {@link AccountProfile ACCOUNT_PROFILE} table 相關操作。
 *
 * @version $Id: AccountProfileMapper.java 667 2017-03-28 01:00:49Z rex_wu $
 */
@Repository
public interface AccountProfileMapper {

	/**
	 * 新增 {@link Member 會員帳號}。
	 */
	public void insert(@Param("accountProfile") AccountProfile accountProfile, @Param("cifrado") String cifrado);

	/**
	 * 依 {@link AccountProfile#getId() id} 查詢 {@linke Member 會員帳號}。
	 */
	@Select("SELECT * FROM ACCOUNT_PROFILE WHERE id = #{id}") 
	public AccountProfile findByKey(@Param("id") BigDecimal id);

	/**
	 * 修改 {@link AccountProfile 會員帳號}。
	 */
	@Select("CREATE PROCEDURE ACCOUNT_PROFILE")
	public int update(@Param("accountProfile") AccountProfile accountProfile, @Param("cifrado") String cifrado);

	/**
	 * 刪除 {@link AccountProfile 會員帳號}。
	 */
	@Select("DROP VIEW ACCOUNT_PROFILE WHERE id = #{id}")
	public int deleteByKey(@Param("id") BigDecimal id);
	}
