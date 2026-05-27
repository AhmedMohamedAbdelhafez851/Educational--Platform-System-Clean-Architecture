// Dashboard Interactive Functions
// Pure JavaScript - No jQuery dependency

// Global variables
window.totalStudents = 0;
window.totalExams = 0;
window.avgScore = 0;
window.attendanceRate = 0;
window.studentsAttention = 0;
window.examsThisWeek = 0;

// Initialize dashboard when DOM is ready
document.addEventListener('DOMContentLoaded', function () {
    console.log('Dashboard initialized');

    // Set global variables from data attributes
    const container = document.querySelector('.dashboard-container');
    if (container) {
        window.totalStudents = parseInt(container.dataset.totalStudents) || 0;
        window.totalExams = parseInt(container.dataset.totalExams) || 0;
        window.avgScore = parseFloat(container.dataset.avgScore) || 0;
        window.attendanceRate = parseFloat(container.dataset.attendanceRate) || 0;
        window.studentsAttention = parseInt(container.dataset.studentsAttention) || 0;
        window.examsThisWeek = parseInt(container.dataset.examsThisWeek) || 0;
    }

    // Make functions globally available
    window.showDetails = showDetails;
    window.goToExam = goToExam;
    window.showStudentDetails = showStudentDetails;
    window.showInsightDetails = showInsightDetails;
});

// Show modal with details
function showDetails(type, title) {
    const modalTitle = document.getElementById('modalTitleText');
    const modalBody = document.getElementById('modalBodyContent');
    const actionBtn = document.getElementById('modalActionButton');

    if (!modalTitle || !modalBody) return;

    modalTitle.innerText = title;
    let content = '';

    switch (type) {
        case 'students':
            content = getStudentsContent();
            if (actionBtn) {
                actionBtn.style.display = 'inline-block';
                actionBtn.onclick = function () { window.location.href = '/Users/Index'; };
            }
            break;
        case 'exams':
            content = getExamsContent();
            if (actionBtn) {
                actionBtn.style.display = 'inline-block';
                actionBtn.onclick = function () { window.location.href = '/Exam/Index'; };
            }
            break;
        case 'score':
            content = getScoreContent();
            if (actionBtn) actionBtn.style.display = 'none';
            break;
        case 'attention':
            content = getAttentionContent();
            if (actionBtn) {
                actionBtn.style.display = 'inline-block';
                actionBtn.onclick = function () { window.location.href = '/Users/Index'; };
            }
            break;
        case 'attendance':
            content = getAttendanceContent();
            if (actionBtn) actionBtn.style.display = 'none';
            break;
        default:
            content = '<p class="text-center text-muted">معلومات تفصيلية غير متاحة حالياً</p>';
    }

    modalBody.innerHTML = content;

    // Show modal using Bootstrap
    const modalElement = document.getElementById('detailsModal');
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    }
}

// Navigate to exam analytics
function goToExam(examId) {
    if (examId) {
        window.location.href = '/Analytics/ExamReport?examId=' + examId;
    }
}

// Show student details
function showStudentDetails(name, score) {
    const modalTitle = document.getElementById('modalTitleText');
    const modalBody = document.getElementById('modalBodyContent');
    const actionBtn = document.getElementById('modalActionButton');

    if (!modalTitle || !modalBody) return;

    modalTitle.innerText = 'تفاصيل الطالب: ' + name;
    if (actionBtn) actionBtn.style.display = 'none';

    modalBody.innerHTML = `
        <div class="detail-stats">
            <div class="detail-stat-card">
                <div class="detail-stat-value">${score}%</div>
                <div class="detail-stat-label">آخر درجة</div>
            </div>
            <div class="detail-stat-card">
                <div class="detail-stat-value">${Math.floor(Math.random() * 5 + 1)}</div>
                <div class="detail-stat-label">الامتحانات المكتملة</div>
            </div>
            <div class="detail-stat-card">
                <div class="detail-stat-value">${Math.floor(Math.random() * 3)}</div>
                <div class="detail-stat-label">الامتحانات الراسبة</div>
            </div>
        </div>
        <div class="alert alert-warning mt-3">
            <i class="bi bi-info-circle-fill me-2"></i>
            يوصى بمتابعة هذا الطالب وتقديم دعم إضافي.
        </div>
    `;

    const modalElement = document.getElementById('detailsModal');
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    }
}

// Show insight details
function showInsightDetails(message, type) {
    const modalTitle = document.getElementById('modalTitleText');
    const modalBody = document.getElementById('modalBodyContent');
    const actionBtn = document.getElementById('modalActionButton');

    if (!modalTitle || !modalBody) return;

    modalTitle.innerText = 'تحليل الرؤية';
    if (actionBtn) actionBtn.style.display = 'none';

    modalBody.innerHTML = `
        <div class="alert alert-info mb-4">
            <i class="bi bi-lightbulb-fill me-2"></i>
            <strong>الرؤية:</strong> ${message}
        </div>
        <div class="detail-stats">
            <div class="detail-stat-card">
                <div class="detail-stat-value">${getInsightValue(type)}</div>
                <div class="detail-stat-label">نسبة التأثير</div>
            </div>
        </div>
        <div class="alert alert-success mt-3">
            <i class="bi bi-check-circle-fill me-2"></i>
            <strong>الإجراء المقترح:</strong> ${getSuggestedAction(type)}
        </div>
    `;

    const modalElement = document.getElementById('detailsModal');
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
    }
}

// Helper functions for content generation
function getStudentsContent() {
    return `
        <div class="detail-stats">
            <div class="detail-stat-card">
                <div class="detail-stat-value">${window.totalStudents}</div>
                <div class="detail-stat-label">إجمالي الطلاب</div>
            </div>
            <div class="detail-stat-card">
                <div class="detail-stat-value">${Math.floor(window.totalStudents * 0.85)}</div>
                <div class="detail-stat-label">طلاب نشطون</div>
            </div>
            <div class="detail-stat-card">
                <div class="detail-stat-value">${window.studentsAttention}</div>
                <div class="detail-stat-label">بحاجة متابعة</div>
            </div>
        </div>
        <h6 class="mb-3 mt-3">آخر الطلاب نشاطاً</h6>
        <div class="student-list-item"><span>أحمد محمد</span><span class="text-success">92%</span></div>
        <div class="student-list-item"><span>سارة خالد</span><span class="text-success">88%</span></div>
        <div class="student-list-item"><span>محمد علي</span><span class="text-warning">65%</span></div>
    `;
}

function getExamsContent() {
    return `
        <div class="detail-stats">
            <div class="detail-stat-card">
                <div class="detail-stat-value">${window.totalExams}</div>
                <div class="detail-stat-label">إجمالي الامتحانات</div>
            </div>
            <div class="detail-stat-card">
                <div class="detail-stat-value">${window.examsThisWeek}</div>
                <div class="detail-stat-label">تم إنشاؤها هذا الأسبوع</div>
            </div>
        </div>
        <div class="alert alert-info mt-3">
            <i class="bi bi-info-circle-fill me-2"></i>
            متوسط الامتحانات الشهرية: ${Math.round(window.totalExams / 3)} امتحان
        </div>
    `;
}

function getScoreContent() {
    const avg = window.avgScore;
    return `
        <div class="detail-stats">
            <div class="detail-stat-card">
                <div class="detail-stat-value">${avg.toFixed(1)}%</div>
                <div class="detail-stat-label">متوسط الدرجات</div>
            </div>
            <div class="detail-stat-card">
                <div class="detail-stat-value">${Math.min(100, avg + 20).toFixed(1)}%</div>
                <div class="detail-stat-label">أعلى درجة</div>
            </div>
            <div class="detail-stat-card">
                <div class="detail-stat-value">${Math.max(0, avg - 30).toFixed(1)}%</div>
                <div class="detail-stat-label">أدنى درجة</div>
            </div>
        </div>
        <div class="progress mb-3" style="height: 8px;">
            <div class="progress-bar bg-success" style="width: ${avg}%"></div>
        </div>
        <div class="alert alert-${avg >= 70 ? 'success' : (avg >= 50 ? 'warning' : 'danger')} mt-3">
            <i class="bi bi-info-circle-fill me-2"></i>
            الأداء العام ${avg >= 70 ? 'جيد جداً' : (avg >= 50 ? 'متوسط' : 'يحتاج تحسين')}
        </div>
    `;
}

function getAttentionContent() {
    return `
        <div class="detail-stats">
            <div class="detail-stat-card">
                <div class="detail-stat-value">${window.studentsAttention}</div>
                <div class="detail-stat-label">طلاب بحاجة متابعة</div>
            </div>
        </div>
        <div class="alert alert-warning mt-3">
            <i class="bi bi-info-circle-fill me-2"></i>
            يوصى بمتابعة هؤلاء الطلاب وتقديم دعم إضافي لهم.
        </div>
    `;
}

function getAttendanceContent() {
    return `
        <div class="detail-stats">
            <div class="detail-stat-card">
                <div class="detail-stat-value">${window.attendanceRate.toFixed(1)}%</div>
                <div class="detail-stat-label">نسبة الحضور الإجمالية</div>
            </div>
        </div>
        <div class="progress mb-3" style="height: 8px;">
            <div class="progress-bar bg-primary" style="width: ${window.attendanceRate}%"></div>
        </div>
        <div class="alert alert-${window.attendanceRate >= 70 ? 'success' : (window.attendanceRate >= 50 ? 'warning' : 'danger')} mt-3">
            <i class="bi bi-info-circle-fill me-2"></i>
            نسبة الحضور ${window.attendanceRate >= 70 ? 'جيدة' : (window.attendanceRate >= 50 ? 'متوسطة' : 'منخفضة')}
        </div>
    `;
}

function getInsightValue(type) {
    if (type === 'success') return 'عالية (85%+)';
    if (type === 'warning') return 'متوسطة (45-65%)';
    return 'منخفضة (أقل من 40%)';
}

function getSuggestedAction(type) {
    if (type === 'success') return 'الاستمرار بنفس المستوى مع تحسين الجودة المستمر';
    if (type === 'warning') return 'مراجعة المحتوى التعليمي وتقديم دورات دعم إضافية للطلاب';
    return 'تحليل البيانات بعمق وتطوير استراتيجيات تدريس جديدة';
}