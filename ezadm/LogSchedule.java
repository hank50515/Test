package com.gss.adm.core.model;

import java.io.Serializable;
import java.util.Date;
import java.util.List;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.OneToMany;
import javax.persistence.Table;

import com.gss.adm.api.enums.ScheduleTypeEnum;
import com.gss.adm.api.enums.SchedulingScanStatusEnum;
import com.gss.adm.api.enums.SchedulingStatusEnum;

import lombok.AccessLevel;
import lombok.EqualsAndHashCode;
import lombok.Getter;
import lombok.Setter;
import lombok.ToString;

/**
 * LogSchedule:
 * 
 * 記錄排程資訊 
 * 
 * @author dennis
 */
@Getter
@Setter
@ToString
@EqualsAndHashCode(of = "id", callSuper = false)
@Entity
@Table(name = "LogSchedule")
public class LogSchedule implements Serializable {

    private static final long serialVersionUID = 1L;

    /** Id */
    @Id
    @Column(name = "id")
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    /** 專案 Id */
    @Column(name = "projectId")
    private Long projectId;

    /** 排程 Id */
    @Column(name = "scheduleId")
    private String scheduleId;
    
    /** 排程名稱 */
    @Column(name = "scheduleName")
    private String scheduleName;

    /** 
     * 排程狀態
     *  
     * P = Pending, S = Started, C = Complete, E = Failure 
     */
    @Column(name = "status", length = 10)
    @Enumerated(EnumType.STRING)
    private SchedulingStatusEnum status;
    
    /** 加入時間 */
    @Column(name = "addingTime", nullable = false)
    private Date addingTime;

    /** 預計開始時間 */
    @Column(name = "estimatedStartTime")
    private Date estimatedStartTime;

    /** 實際開始時間 */
    @Column(name = "startTime", nullable = true)
    private Date startTime;

    /** 實際完成時間 */
    @Column(name = "completeTime", nullable = true)
    private Date completeTime;
    
    /** 所花費的時間 */
    @Column(name = "timespent", nullable = true)
    private Long timespent;
    
    /** 備註 */
    @Column(name = "comment", nullable = true, length = 255)
    private String comment;
    
    /** 排程型態 */
    @Column(name = "scheduleType", nullable = false, length = 255)
    @Enumerated(EnumType.STRING)
    private ScheduleTypeEnum scheduleType;
    
    @Setter(AccessLevel.NONE)
    @JoinColumn(name = "logId")
    @OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)
    private List<LogScheduleStep> steps;
    
    /** 排程執行是 incremental or full */
    @Column(name = "scanStatus", length = 50)
    @Enumerated(EnumType.STRING)
    private SchedulingScanStatusEnum scanStatus;
}
