



export default function Page() {
    let date = new Date();
    let year = date.getFullYear();
    let moon = "🌙";

    return (
        <div className="terms-of-service">
            <h1>Terms of Service</h1>
            <p>Welcome to our Terms of Service page. Please read these terms carefully before using our service.</p>
            <h2>1. Acceptance of Terms</h2>
            <p>By accessing or using our service, you agree to be bound by these terms.</p>
            <h2>2. Changes to Terms</h2>
            <p>We may modify these terms at any time. Your continued use of the service after changes are made constitutes your acceptance of the new terms.</p>
            <h2>3. User Responsibilities</h2>
            <p>You are responsible for your use of the service and for any content you post.</p>
            <h2>4. Limitation of Liability</h2>
            <p>We are not liable for any damages arising from your use of the service.</p>
            <br/>

            <p>Last updated: {year} {moon}</p>
            <p>By using our service, you agree to these terms. If you do not agree, please do not use our service.</p>
        </div>
    );
}
