// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem('jwtToken') || sessionStorage.getItem('jwtToken');
    const loginButton = document.getElementById('login-button');
    const userMenu = document.getElementById('user-menu');

    if (token) {
        try {
            // 1. Giải mã token để lấy thông tin user
            const payload = JSON.parse(atob(token.split('.')[1]));

            // Lấy thông tin từ payload (tên claim có thể khác nhau tùy vào cách bạn tạo token)
            // Common claims: "unique_name" (username), "email", "given_name" (fullname)
            const userName = payload.unique_name || payload.given_name || "User";
            const userAvatar = payload.avatar || "https://via.placeholder.com/40"; // Giả sử có claim 'avatar'

            // 2. Cập nhật giao diện
            document.getElementById('user-name-span').textContent = userName;
            document.getElementById('user-avatar-img').src = userAvatar;

            // 3. Ẩn/hiện các nút
            if (loginButton) loginButton.style.display = 'none';
            if (userMenu) userMenu.style.display = 'block';

        } catch (e) {
            console.error("Invalid token:", e);
            // Nếu token không hợp lệ, xóa nó đi
            localStorage.removeItem('jwtToken');
            sessionStorage.removeItem('jwtToken');
        }
    }

    // Xử lý nút Đăng xuất
    const logoutButton = document.getElementById('logout-button');
    if (logoutButton) {
        logoutButton.addEventListener('click', function (e) {
            e.preventDefault();

            // Xóa token khỏi cả hai nơi
            localStorage.removeItem('jwtToken');
            sessionStorage.removeItem('jwtToken');

            // Tải lại trang để giao diện cập nhật về trạng thái chưa đăng nhập
            window.location.href = '/';
        });
    }
});


// =================================================================
// HERO SECTION SLIDESHOW LOGIC
// =================================================================
document.addEventListener('DOMContentLoaded', function () {
    const heroSection = document.querySelector('.hero-section');
    // Chỉ chạy mã này nếu có hero-section trên trang
    if (!heroSection) return;

    const slides = [
        {
            id: 1,
            image: '/images/background_PY_1.jpg', // Đảm bảo đường dẫn đúng
            title: "PHÚ YÊN",
            subtitle: "Xứ sở hoa vàng cỏ xanh"
        },
        {
            id: 2,
            image: '/images/background_GDD_3.jpg', // Đổi tên file cho đúng
            title: "GÀNH ĐÁ ĐĨA",
            subtitle: "Kỳ quan thiên nhiên độc đáo"
        },
        {
            id: 3,
            image: '/images/background_BX_2.jpg', // Đổi tên file cho đúng
            title: "BÃI XÉP",
            subtitle: "Thiên đường nghỉ dưỡng"
        },
        {
            id: 4,
            image: '/images/background_MD_4.jpg', // Đảm bảo đường dẫn đúng
            title: "MŨI ĐIỆN",
            subtitle: "Đón ánh bình minh đầu tiên"
        },
    ];

    let currentSlide = 0;
    const slideshowContainer = document.querySelector('.hero-slideshow');
    const titleElement = document.getElementById('hero-title');
    const subtitleElement = document.getElementById('hero-subtitle');
    const dotsContainer = document.getElementById('slide-dots');

    function buildSlideshow() {
        slideshowContainer.innerHTML = '';
        dotsContainer.innerHTML = '';

        slides.forEach((slide, index) => {
            // Tạo slide div
            const slideDiv = document.createElement('div');
            slideDiv.className = 'hero-slide';
            slideDiv.style.backgroundImage = `linear-gradient(rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.4)), url(${slide.image})`;
            slideshowContainer.appendChild(slideDiv);

            // Tạo dot span
            const dotSpan = document.createElement('span');
            dotSpan.className = 'dot';
            dotSpan.addEventListener('click', () => goToSlide(index));
            dotsContainer.appendChild(dotSpan);
        });
    }

    function updateSlideContent() {
        if (!titleElement || !subtitleElement) return;

        // Cập nhật text
        titleElement.textContent = slides[currentSlide].title;
        subtitleElement.textContent = slides[currentSlide].subtitle;

        // Cập nhật class 'active' cho slide và dot
        const slideDivs = slideshowContainer.querySelectorAll('.hero-slide');
        const dotSpans = dotsContainer.querySelectorAll('.dot');

        slideDivs.forEach((div, index) => {
            div.classList.toggle('active', index === currentSlide);
        });

        dotSpans.forEach((span, index) => {
            span.classList.toggle('active', index === currentSlide);
        });
    }

    function goToSlide(index) {
        currentSlide = index;
        updateSlideContent();
    }

    function nextSlide() {
        currentSlide = (currentSlide + 1) % slides.length;
        updateSlideContent();
    }

    // Khởi tạo slideshow
    buildSlideshow();
    updateSlideContent();

    // Tự động chuyển slide
    setInterval(nextSlide, 5000);
});




// =================================================================
// TRAVEL PLAN SECTION LOGIC
// =================================================================
document.addEventListener('DOMContentLoaded', function () {
    const travelPlanSection = document.querySelector('.travel-plan-section');
    if (!travelPlanSection) return;

    const steps = [
        { id: 1, icon: 'calendar-event', title: 'Chọn thời gian', description: 'Lựa chọn thời điểm lý tưởng để du lịch Phú Yên và tham khảo thông tin về mùa đẹp nhất trong năm.' },
        { id: 2, icon: 'pin-map', title: 'Chọn địa điểm', description: 'Khám phá và lựa chọn các điểm đến phù hợp với sở thích và thời gian của bạn tại Phú Yên.' },
        { id: 3, icon: 'houses', title: 'Chọn nơi nghỉ', description: 'Tìm kiếm các khách sạn, homestay hoặc resort phù hợp với ngân sách và trải nghiệm bạn mong muốn.' },
        { id: 4, icon: 'journal-check', title: 'Lên lịch trình', description: 'Sắp xếp các địa điểm và hoạt động vào lịch trình chi tiết, tối ưu thời gian di chuyển.' }
    ];

    let activeStep = 1;

    const timelinePointsWrapper = document.getElementById('timeline-points-wrapper');
    const stepCardsContainer = document.getElementById('step-cards-container');
    const timelineProgress = document.getElementById('timeline-progress');
    const btnPrev = document.getElementById('btn-prev');
    const btnNext = document.getElementById('btn-next');
    const btnCreatePlan = document.getElementById('btn-create-plan');
    const stepNavigation = document.querySelector('.step-navigation');

    function buildStepUI() {
        timelinePointsWrapper.innerHTML = '';
        // Xóa các thẻ cũ trừ phần navigation
        Array.from(stepCardsContainer.children).forEach(child => {
            if (!child.classList.contains('step-navigation')) {
                child.remove();
            }
        });

        steps.forEach(step => {
            // Build timeline points
            const pointDiv = document.createElement('div');
            pointDiv.className = 'timeline-point';
            pointDiv.dataset.stepId = step.id;
            pointDiv.innerHTML = `<div class="point-number">${step.id}</div><div class="point-label">${step.title}</div>`;
            pointDiv.addEventListener('click', () => goToStep(step.id));
            timelinePointsWrapper.appendChild(pointDiv);

            // Build step cards
            const cardDiv = document.createElement('div');
            cardDiv.className = 'step-card';
            cardDiv.dataset.stepId = step.id;
            cardDiv.innerHTML = `
                <div class="step-icon"><i class="bi bi-${step.icon}"></i></div>
                <div class="step-content">
                    <h3 class="step-title">${step.title}</h3>
                    <p class="step-description">${step.description}</p>
                </div>
            `;
            // Chèn card vào trước phần navigation
            stepCardsContainer.insertBefore(cardDiv, stepNavigation);
        });
    }

    function updateUI() {
        // Update progress bar
        timelineProgress.style.width = `${(activeStep - 1) * 100 / (steps.length - 1)}%`;

        // Update timeline points
        document.querySelectorAll('.timeline-point').forEach(point => {
            point.classList.toggle('active', parseInt(point.dataset.stepId) <= activeStep);
        });

        // Update step cards
        document.querySelectorAll('.step-card').forEach(card => {
            card.classList.toggle('active', parseInt(card.dataset.stepId) === activeStep);
        });

        // Update navigation buttons
        btnPrev.disabled = activeStep === 1;
        btnNext.style.display = activeStep < steps.length ? 'inline-flex' : 'none';
        btnCreatePlan.style.display = activeStep === steps.length ? 'inline-flex' : 'none';
    }

    function goToStep(stepId) {
        activeStep = stepId;
        updateUI();
    }

    btnPrev.addEventListener('click', () => {
        if (activeStep > 1) {
            activeStep--;
            updateUI();
        }
    });

    btnNext.addEventListener('click', () => {
        if (activeStep < steps.length) {
            activeStep++;
            updateUI();
        }
    });

    // Initial setup
    buildStepUI();
    updateUI();
});


// =================================================================
// TESTIMONIALS SECTION LOGIC
// =================================================================
document.addEventListener('DOMContentLoaded', function () {
    const testimonialsSection = document.querySelector('.testimonials-section');
    if (!testimonialsSection) return;

    const testimonials = [
        { id: 1, author: "Nguyễn Thanh Hà", position: "Blogger Du lịch", avatar: '/images/avatar-1.jpg', rating: 5, content: "Phú Yên là một địa điểm du lịch tuyệt vời với những bãi biển hoang sơ, đặc biệt là Bãi Xép - nơi tôi đã có những bức ảnh đẹp nhất trong chuyến đi. Ẩm thực địa phương phong phú, hải sản tươi ngon và giá cả phải chăng.", date: "Tháng 3, 2025" },
        { id: 2, author: "Trần Minh Đức", position: "Nhiếp ảnh gia", avatar: '/images/avatar-2.jpg', rating: 5, content: "Gành Đá Đĩa là một kỳ quan thiên nhiên độc đáo mà bất kỳ ai đến Phú Yên đều phải ghé thăm. Những khối đá xếp chồng lên nhau tạo nên một khung cảnh ngoạn mục, đặc biệt khi hoàng hôn. Tôi đã có những tấm hình tuyệt vời tại đây!", date: "Tháng 5, 2025" },
        { id: 3, author: "Lê Thị Phương", position: "Doanh nhân", avatar: '/images/avatar-3.jpg', rating: 4, content: "Chuyến đi Phú Yên là một quyết định tuyệt vời cho kỳ nghỉ ngắn ngày của gia đình tôi. Con tôi thích thú với biển Bãi Xép và những hoạt động thú vị tại đây. Mọi người đều rất thân thiện và nhiệt tình giúp đỡ du khách.", date: "Tháng 2, 2025" },
        { id: 4, author: "Hoàng Văn Nam", position: "Kỹ sư phần mềm", avatar: '/images/avatar-4.jpg', rating: 5, content: "Mũi Điện với ngọn hải đăng cổ là điểm đến không thể bỏ qua khi đến Phú Yên. Được đón bình minh đầu tiên trên đất liền tại đây là một trải nghiệm tuyệt vời. Nhà nghỉ ở đây sạch sẽ, giá cả hợp lý và dịch vụ rất tốt.", date: "Tháng 6, 2025" }
    ];

    let activeIndex = 0;
    let autoPlayTimeout;
    const slider = document.getElementById('testimonials-slider');
    const dotsContainer = document.getElementById('testimonial-dots');
    const btnPrev = document.getElementById('testimonial-prev');
    const btnNext = document.getElementById('testimonial-next');

    function renderStars(rating) {
        let starsHtml = '';
        for (let i = 0; i < 5; i++) {
            starsHtml += `<i class="bi ${i < rating ? 'bi-star-fill' : 'bi-star'}"></i>`;
        }
        return `<div class="testimonial-rating">${starsHtml}</div>`;
    }

    function buildTestimonials() {
        slider.innerHTML = '';
        dotsContainer.innerHTML = '';

        testimonials.forEach((item, index) => {
            // Build slides
            const slide = document.createElement('div');
            slide.className = 'testimonial-item';
            slide.innerHTML = `
                <div class="testimonial-content">
                    <p class="testimonial-text">${item.content}</p>
                    <div class="testimonial-meta">
                        ${renderStars(item.rating)}
                        <div class="testimonial-date">${item.date}</div>
                    </div>
                </div>
                <div class="testimonial-author">
                    <div class="author-avatar"><img src="${item.avatar}" alt="${item.author}" /></div>
                    <div class="author-info">
                        <h4 class="author-name">${item.author}</h4>
                        <p class="author-position">${item.position}</p>
                    </div>
                </div>
            `;
            slider.appendChild(slide);

            // Build dots
            const dot = document.createElement('button');
            dot.className = 'testimonial-dot';
            dot.setAttribute('aria-label', `Go to slide ${index + 1}`);
            dot.addEventListener('click', () => {
                goToSlide(index);
                resetAutoPlay();
            });
            dotsContainer.appendChild(dot);
        });
    }

    function updateActiveSlide() {
        const slides = slider.querySelectorAll('.testimonial-item');
        const dots = dotsContainer.querySelectorAll('.testimonial-dot');
        slides.forEach((slide, index) => {
            slide.classList.toggle('active', index === activeIndex);
        });
        dots.forEach((dot, index) => {
            dot.classList.toggle('active', index === activeIndex);
        });
    }

    function goToSlide(index) {
        activeIndex = index;
        updateActiveSlide();
    }

    function resetAutoPlay() {
        clearTimeout(autoPlayTimeout);
        autoPlayTimeout = setTimeout(() => goToSlide((activeIndex + 1) % testimonials.length), 5000);
    }

    btnPrev.addEventListener('click', () => {
        goToSlide((activeIndex - 1 + testimonials.length) % testimonials.length);
        resetAutoPlay();
    });

    btnNext.addEventListener('click', () => {
        goToSlide((activeIndex + 1) % testimonials.length);
        resetAutoPlay();
    });

    // Initial setup
    buildTestimonials();
    updateActiveSlide();
    resetAutoPlay();
});
