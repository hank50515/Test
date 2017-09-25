package tw.gov.tipo.patkm.file.mapper;
　
import org.apache.ibatis.annotations.Param;
import org.springframework.stereotype.Repository;
　
import tw.gov.tipo.patkm.file.entity.SystemProperty;
　
/**
 * {@link SystemProperty SYSTEM_PROPERTY} table 相關操作。
 *
 * @version $Id: SystemPropertyMapper.java 330 2017-02-09 06:43:57Z rex_wu $
 */
@Repository
public interface SystemPropertyMapper {
    /**
     * 依 {@link SystemProperty#getSystem() system} 及 {@link SystemProperty#getName() name} 查詢 {@link SystemProperty#getValue() 系統屬性值}。
     */
    public String findValueBy(@Param("system") String system, @Param("name") String name);
}